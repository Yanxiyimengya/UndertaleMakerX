using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using Jint;
using Jint.Native;
using Jint.Native.Json;

public class JavaScriptFileAccess
{
    private FileStream _stream;

    private JavaScriptFileAccess()
    {
    }

    private JavaScriptFileAccess(FileStream stream)
    {
        _stream = stream;
    }

    private static Engine engine => JavaScriptBridge.MainEngine;

    private static JsValue jsNull() => JsValue.Null;
    private static JsValue jsUndefined() => JsValue.Undefined;

    private static JsValue jsFrom(object obj)
    {
        try
        {
            var e = engine;
            if (e == null) return JsValue.Null;
            return JsValue.FromObject(e, obj);
        }
        catch
        {
            return JsValue.Null;
        }
    }

    private static bool isNullOrUndefined(JsValue v) => v.IsNull() || v.IsUndefined();

    private static string toStringSafe(JsValue v)
    {
        try
        {
            if (isNullOrUndefined(v)) return null;
            if (v.IsString()) return v.AsString();
            return v.ToString();
        }
        catch
        {
            return null;
        }
    }

    private static int toInt32Safe(JsValue v, int defaultValue = 0)
    {
        try
        {
            if (isNullOrUndefined(v)) return defaultValue;
            if (v.IsNumber()) return (int)v.AsNumber();
            var s = v.ToString();
            if (int.TryParse(s, out var n)) return n;
            return defaultValue;
        }
        catch
        {
            return defaultValue;
        }
    }

    private static long toInt64Safe(JsValue v, long defaultValue = 0)
    {
        try
        {
            if (isNullOrUndefined(v)) return defaultValue;
            if (v.IsNumber()) return (long)v.AsNumber();
            var s = v.ToString();
            if (long.TryParse(s, out var n)) return n;
            return defaultValue;
        }
        catch
        {
            return defaultValue;
        }
    }

    private static bool toBoolSafe(JsValue v, bool defaultValue = false)
    {
        try
        {
            if (isNullOrUndefined(v)) return defaultValue;
            if (v.IsBoolean()) return v.AsBoolean();
            if (v.IsNumber()) return v.AsNumber() != 0;
            if (v.IsString())
            {
                var s = v.AsString();
                if (string.Equals(s, "true", StringComparison.OrdinalIgnoreCase)) return true;
                if (string.Equals(s, "false", StringComparison.OrdinalIgnoreCase)) return false;
                if (int.TryParse(s, out var n)) return n != 0;
            }
            return defaultValue;
        }
        catch
        {
            return defaultValue;
        }
    }

    private static string normalizePathInternal(string path)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(path)) return null;
            var p = path.Replace("\\", "/");
            return Path.GetFullPath(p).Replace("\\", "/");
        }
        catch
        {
            return path;
        }
    }

    private static Encoding parseEncoding(JsValue options)
    {
        try
        {
            if (isNullOrUndefined(options) || !options.IsObject()) return Encoding.UTF8;
            var obj = options.AsObject();
            var enc = obj.Get("encoding");
            var encName = toStringSafe(enc);
            if (string.IsNullOrWhiteSpace(encName)) return Encoding.UTF8;
            return Encoding.GetEncoding(encName);
        }
        catch
        {
            return Encoding.UTF8;
        }
    }

    private static int parseIndent(JsValue options, int defaultIndent = 2)
    {
        try
        {
            if (isNullOrUndefined(options) || !options.IsObject()) return defaultIndent;
            var obj = options.AsObject();
            var indent = obj.Get("indent");
            var i = toInt32Safe(indent, defaultIndent);
            if (i < 0) i = 0;
            if (i > 16) i = 16;
            return i;
        }
        catch
        {
            return defaultIndent;
        }
    }

    private static bool getPath(string method, JsValue path, out string norm)
    {
        norm = null;
        var p = toStringSafe(path);
        if (string.IsNullOrWhiteSpace(p))
        {
            UtmxLogger.Warning(method, "path is empty");
            return false;
        }
        norm = normalizePathInternal(p);
        if (string.IsNullOrWhiteSpace(norm))
        {
            UtmxLogger.Warning(method, "path normalize failed");
            return false;
        }
        return true;
    }

    private static JsValue tryRun(string method, Func<JsValue> fn)
    {
        try
        {
            return fn();
        }
        catch (Exception e)
        {
            UtmxLogger.Error(method, e.Message, e.StackTrace);
            return jsNull();
        }
    }

    private static JsValue tryRunBool(string method, Func<bool> fn)
    {
        return tryRun(method, () => jsFrom(fn()));
    }

    private void ensureOpen()
    {
        if (_stream == null) throw new InvalidOperationException("File handle is null/closed.");
    }

    private static (FileMode fm, FileAccess fa) parseMode(string mode)
    {
        if (string.IsNullOrWhiteSpace(mode)) mode = "r";
        switch (mode)
        {
            case "r": return (FileMode.Open, FileAccess.Read);
            case "w": return (FileMode.Create, FileAccess.Write);
            case "rw":
            case "wr": return (FileMode.OpenOrCreate, FileAccess.ReadWrite);
            default: return (FileMode.Open, FileAccess.Read);
        }
    }

    private static byte[] jsArrayToBytes(JsValue data)
    {
        try
        {
            if (isNullOrUndefined(data)) return null;

            if (data.IsArray())
            {
                var arr = data.AsArray();
                var len = (int)arr.Length;
                var bytes = new byte[len];
                for (int i = 0; i < len; i++)
                {
                    var v = arr.Get(i);
                    int n = 0;
                    if (!isNullOrUndefined(v))
                    {
                        if (v.IsNumber()) n = (int)v.AsNumber();
                        else int.TryParse(v.ToString(), out n);
                    }
                    if (n < 0) n = 0;
                    if (n > 255) n = 255;
                    bytes[i] = (byte)n;
                }
                return bytes;
            }

            var obj = data.ToObject();
            if (obj is byte[] b) return b;
            return null;
        }
        catch
        {
            return null;
        }
    }

    public static JsValue openFile(JsValue path, JsValue mode)
    {
        return tryRun("fileAccess.openFile", () =>
        {
            if (!getPath("fileAccess.openFile", path, out var norm)) return jsNull();

            var m = toStringSafe(mode);
            var (fm, fa) = parseMode(m);

            FileStream fs;
            try
            {
                fs = new FileStream(norm, fm, fa, FileShare.ReadWrite);
            }
            catch (Exception e)
            {
                UtmxLogger.Error("fileAccess.openFile", norm, m, e.Message, e.StackTrace);
                return jsNull();
            }

            var handle = new JavaScriptFileAccess(fs);
            return jsFrom(handle);
        });
    }

    public JsValue close()
    {
        return tryRunBool("fileAccess.close", () =>
        {
            if (_stream == null) return true;
            try { _stream.Flush(); } catch { }
            try { _stream.Dispose(); } catch { }
            _stream = null;
            return true;
        });
    }

    public JsValue flush()
    {
        return tryRunBool("fileAccess.flush", () =>
        {
            ensureOpen();
            _stream.Flush();
            return true;
        });
    }

    public JsValue tell()
    {
        return tryRun("fileAccess.tell", () =>
        {
            ensureOpen();
            return jsFrom((double)_stream.Position);
        });
    }

    public JsValue seek(JsValue position, JsValue origin)
    {
        return tryRun("fileAccess.seek", () =>
        {
            ensureOpen();
            var pos = toInt64Safe(position, 0);
            var ori = toStringSafe(origin);

            SeekOrigin so = SeekOrigin.Begin;
            if (!string.IsNullOrWhiteSpace(ori))
            {
                if (ori == "begin") so = SeekOrigin.Begin;
                else if (ori == "current") so = SeekOrigin.Current;
                else if (ori == "end") so = SeekOrigin.End;
            }

            _stream.Seek(pos, so);
            return jsFrom(this);
        });
    }

    public JsValue read(JsValue size)
    {
        return tryRun("fileAccess.read", () =>
        {
            ensureOpen();

            int n = toInt32Safe(size, -1);
            if (n <= 0)
            {
                long remaining = _stream.Length - _stream.Position;
                if (remaining <= 0) return jsFrom(Array.Empty<byte>());
                if (remaining > int.MaxValue) remaining = int.MaxValue;
                n = (int)remaining;
            }

            var buf = new byte[n];
            int read = _stream.Read(buf, 0, n);
            if (read == n) return jsFrom(buf);

            var actual = new byte[read];
            Buffer.BlockCopy(buf, 0, actual, 0, read);
            return jsFrom(actual);
        });
    }

    public JsValue write(JsValue data)
    {
        return tryRun("fileAccess.write", () =>
        {
            ensureOpen();

            if (isNullOrUndefined(data))
            {
                UtmxLogger.Warning("fileAccess.write", "data is null/undefined");
                return jsFrom(this);
            }

            if (data.IsString())
            {
                var s = data.AsString() ?? "";
                var bytes = Encoding.UTF8.GetBytes(s);
                _stream.Write(bytes, 0, bytes.Length);
                return jsFrom(this);
            }

            var b = jsArrayToBytes(data);
            if (b != null)
            {
                _stream.Write(b, 0, b.Length);
                return jsFrom(this);
            }

            var text = data.ToString();
            var bb = Encoding.UTF8.GetBytes(text ?? "");
            _stream.Write(bb, 0, bb.Length);
            return jsFrom(this);
        });
    }

    public JsValue readLine()
    {
        return tryRun("fileAccess.readLine", () =>
        {
            ensureOpen();
            if (_stream.Position >= _stream.Length) return jsNull();

            List<byte> bytes = new List<byte>(128);

            while (_stream.Position < _stream.Length)
            {
                int b = _stream.ReadByte();
                if (b < 0) break;

                if (b == '\n') break;
                if (b == '\r')
                {
                    if (_stream.Position < _stream.Length)
                    {
                        int next = _stream.ReadByte();
                        if (next != '\n' && next >= 0) _stream.Position -= 1;
                    }
                    break;
                }

                bytes.Add((byte)b);
            }

            var line = Encoding.UTF8.GetString(bytes.ToArray());
            return jsFrom(line);
        });
    }

    public JsValue readToEnd()
    {
        return tryRun("fileAccess.readToEnd", () =>
        {
            ensureOpen();

            long remaining = _stream.Length - _stream.Position;
            if (remaining <= 0) return jsFrom("");

            if (remaining > int.MaxValue) remaining = int.MaxValue;
            int n = (int)remaining;

            var buf = new byte[n];
            int read = _stream.Read(buf, 0, n);

            var text = Encoding.UTF8.GetString(buf, 0, read);
            return jsFrom(text);
        });
    }

    public JsValue fileExists(JsValue path)
    {
        return tryRun("fileAccess.fileExists", () =>
        {
            if (!getPath("fileAccess.fileExists", path, out var norm)) return jsFrom(false);
            return jsFrom(File.Exists(norm));
        });
    }

    public JsValue directoryExists(JsValue path)
    {
        return tryRun("fileAccess.directoryExists", () =>
        {
            if (!getPath("fileAccess.directoryExists", path, out var norm)) return jsFrom(false);
            return jsFrom(Directory.Exists(norm));
        });
    }

    public JsValue readAllText(JsValue path)
    {
        return tryRun("fileAccess.readAllText", () =>
        {
            if (!getPath("fileAccess.readAllText", path, out var norm)) return jsNull();
            var content = File.ReadAllText(norm, Encoding.UTF8);
            return jsFrom(content);
        });
    }

    public JsValue writeAllText(JsValue path, JsValue content)
    {
        return tryRunBool("fileAccess.writeAllText", () =>
        {
            if (!getPath("fileAccess.writeAllText", path, out var norm)) return false;
            var c = toStringSafe(content) ?? "";
            File.WriteAllText(norm, c, Encoding.UTF8);
            return true;
        });
    }

    public JsValue appendText(JsValue path, JsValue content)
    {
        return tryRunBool("fileAccess.appendText", () =>
        {
            if (!getPath("fileAccess.appendText", path, out var norm)) return false;
            var c = toStringSafe(content) ?? "";
            File.AppendAllText(norm, c, Encoding.UTF8);
            return true;
        });
    }

    public JsValue readAllBytes(JsValue path)
    {
        return tryRun("fileAccess.readAllBytes", () =>
        {
            if (!getPath("fileAccess.readAllBytes", path, out var norm)) return jsNull();
            var bytes = File.ReadAllBytes(norm);
            return jsFrom(bytes);
        });
    }

    public JsValue writeAllBytes(JsValue path, JsValue data)
    {
        return tryRunBool("fileAccess.writeAllBytes", () =>
        {
            if (!getPath("fileAccess.writeAllBytes", path, out var norm)) return false;

            byte[] bytes = jsArrayToBytes(data);
            if (bytes == null)
            {
                var s = toStringSafe(data);
                bytes = Encoding.UTF8.GetBytes(s ?? "");
            }

            File.WriteAllBytes(norm, bytes ?? Array.Empty<byte>());
            return true;
        });
    }

    public JsValue copyFile(JsValue source, JsValue dest, JsValue overwrite)
    {
        return tryRunBool("fileAccess.copyFile", () =>
        {
            var s = toStringSafe(source);
            var d = toStringSafe(dest);
            var ow = toBoolSafe(overwrite, false);

            if (string.IsNullOrWhiteSpace(s) || string.IsNullOrWhiteSpace(d))
            {
                UtmxLogger.Warning("fileAccess.copyFile", "source/dest is empty");
                return false;
            }

            var ns = normalizePathInternal(s);
            var nd = normalizePathInternal(d);

            File.Copy(ns, nd, ow);
            return true;
        });
    }

    public JsValue moveFile(JsValue source, JsValue dest)
    {
        return tryRunBool("fileAccess.moveFile", () =>
        {
            var s = toStringSafe(source);
            var d = toStringSafe(dest);

            if (string.IsNullOrWhiteSpace(s) || string.IsNullOrWhiteSpace(d))
            {
                UtmxLogger.Warning("fileAccess.moveFile", "source/dest is empty");
                return false;
            }

            var ns = normalizePathInternal(s);
            var nd = normalizePathInternal(d);

            File.Move(ns, nd);
            return true;
        });
    }

    public JsValue deleteFile(JsValue path)
    {
        return tryRunBool("fileAccess.deleteFile", () =>
        {
            if (!getPath("fileAccess.deleteFile", path, out var norm)) return false;
            if (!File.Exists(norm)) return true;
            File.Delete(norm);
            return true;
        });
    }

    public JsValue createDirectory(JsValue path)
    {
        return tryRunBool("fileAccess.createDirectory", () =>
        {
            if (!getPath("fileAccess.createDirectory", path, out var norm)) return false;
            Directory.CreateDirectory(norm);
            return true;
        });
    }

    public JsValue deleteDirectory(JsValue path, JsValue recursive)
    {
        return tryRunBool("fileAccess.deleteDirectory", () =>
        {
            if (!getPath("fileAccess.deleteDirectory", path, out var norm)) return false;
            var rec = toBoolSafe(recursive, false);
            if (!Directory.Exists(norm)) return true;
            Directory.Delete(norm, rec);
            return true;
        });
    }

    public JsValue getFiles(JsValue path, JsValue pattern)
    {
        return tryRun("fileAccess.getFiles", () =>
        {
            if (!getPath("fileAccess.getFiles", path, out var norm)) return jsNull();

            var pat = toStringSafe(pattern);
            if (string.IsNullOrWhiteSpace(pat)) pat = "*";
            if (!Directory.Exists(norm)) return jsFrom(Array.Empty<string>());

            var files = Directory.GetFiles(norm, pat, SearchOption.TopDirectoryOnly)
                .Select(x => x.Replace("\\", "/"))
                .ToArray();

            return jsFrom(files);
        });
    }

    public JsValue getDirectories(JsValue path, JsValue pattern)
    {
        return tryRun("fileAccess.getDirectories", () =>
        {
            if (!getPath("fileAccess.getDirectories", path, out var norm)) return jsNull();

            var pat = toStringSafe(pattern);
            if (string.IsNullOrWhiteSpace(pat)) pat = "*";
            if (!Directory.Exists(norm)) return jsFrom(Array.Empty<string>());

            var dirs = Directory.GetDirectories(norm, pat, SearchOption.TopDirectoryOnly)
                .Select(x => x.Replace("\\", "/"))
                .ToArray();

            return jsFrom(dirs);
        });
    }

    public JsValue listFiles(JsValue path, JsValue pattern) => getFiles(path, pattern);

    public JsValue getFileSize(JsValue path)
    {
        return tryRun("fileAccess.getFileSize", () =>
        {
            if (!getPath("fileAccess.getFileSize", path, out var norm)) return jsNull();
            if (!File.Exists(norm)) return jsNull();
            var size = new FileInfo(norm).Length;
            return jsFrom((double)size);
        });
    }

    public JsValue getCreationTime(JsValue path)
    {
        return tryRun("fileAccess.getCreationTime", () =>
        {
            if (!getPath("fileAccess.getCreationTime", path, out var norm)) return jsNull();
            if (!File.Exists(norm)) return jsNull();
            var t = File.GetCreationTimeUtc(norm);
            return jsFrom(t.ToString("o"));
        });
    }

    public JsValue getLastWriteTime(JsValue path)
    {
        return tryRun("fileAccess.getLastWriteTime", () =>
        {
            if (!getPath("fileAccess.getLastWriteTime", path, out var norm)) return jsNull();
            if (!File.Exists(norm)) return jsNull();
            var t = File.GetLastWriteTimeUtc(norm);
            return jsFrom(t.ToString("o"));
        });
    }

    public JsValue normalizePath(JsValue path)
    {
        return tryRun("fileAccess.normalizePath", () =>
        {
            var p = toStringSafe(path);
            if (string.IsNullOrWhiteSpace(p)) return jsNull();
            return jsFrom(normalizePathInternal(p));
        });
    }

    public JsValue combinePaths(JsValue paths)
    {
        return tryRun("fileAccess.combinePaths", () =>
        {
            if (isNullOrUndefined(paths))
            {
                UtmxLogger.Warning("fileAccess.combinePaths", "paths is null/undefined");
                return jsNull();
            }

            List<string> parts = new List<string>();

            if (paths.IsArray())
            {
                var arr = paths.AsArray();
                var len = (int)arr.Length;
                for (int i = 0; i < len; i++)
                {
                    var s = toStringSafe(arr.Get(i));
                    if (!string.IsNullOrWhiteSpace(s)) parts.Add(s);
                }
            }
            else
            {
                var s = toStringSafe(paths);
                if (!string.IsNullOrWhiteSpace(s)) parts.Add(s);
            }

            if (parts.Count == 0) return jsNull();

            var combined = parts[0];
            for (int i = 1; i < parts.Count; i++)
                combined = Path.Combine(combined, parts[i]);

            combined = combined.Replace("\\", "/");
            return jsFrom(combined);
        });
    }

    public JsValue getTempFileName()
    {
        return tryRun("fileAccess.getTempFileName", () =>
        {
            var p = Path.GetTempFileName().Replace("\\", "/");
            return jsFrom(p);
        });
    }

    public JsValue getTempPath()
    {
        return tryRun("fileAccess.getTempPath", () =>
        {
            var p = Path.GetTempPath().Replace("\\", "/");
            return jsFrom(p);
        });
    }

    public JsValue getFileHash(JsValue path, JsValue algo)
    {
        return tryRun("fileAccess.getFileHash", () =>
        {
            if (!getPath("fileAccess.getFileHash", path, out var norm)) return jsNull();
            if (!File.Exists(norm)) return jsNull();

            var a = toStringSafe(algo);
            if (string.IsNullOrWhiteSpace(a)) a = "md5";
            a = a.ToLowerInvariant();

            byte[] hash;
            using (var stream = File.OpenRead(norm))
            {
                if (a == "sha1")
                {
                    using var h = SHA1.Create();
                    hash = h.ComputeHash(stream);
                }
                else if (a == "sha256")
                {
                    using var h = SHA256.Create();
                    hash = h.ComputeHash(stream);
                }
                else
                {
                    using var h = MD5.Create();
                    hash = h.ComputeHash(stream);
                }
            }

            var hex = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            return jsFrom(hex);
        });
    }

    public JsValue readJson(JsValue path, JsValue options)
    {
        return tryRun("fileAccess.readJson", () =>
        {
            if (!getPath("fileAccess.readJson", path, out var norm)) return jsNull();

            var enc = parseEncoding(options);
            var json = File.ReadAllText(norm, enc);
            if (string.IsNullOrWhiteSpace(json)) return jsNull();

            var e = engine;
            if (e == null) return jsNull();

            var parser = new JsonParser(e);
            return parser.Parse(json);
        });
    }

    public JsValue writeJson(JsValue path, JsValue obj, JsValue options)
    {
        return tryRunBool("fileAccess.writeJson", () =>
        {
            if (!getPath("fileAccess.writeJson", path, out var norm)) return false;

            var enc = parseEncoding(options);
            var indent = parseIndent(options, 2);

            var e = engine;
            if (e == null) return false;

            var serializer = new JsonSerializer(e);
            var space = indent > 0 ? new Jint.Native.JsString(new string(' ', indent)) : JsValue.Undefined;
            var json = serializer.Serialize(obj, JsValue.Undefined, space).AsString();

            File.WriteAllText(norm, json ?? "null", enc);
            return true;
        });
    }

    public JsValue isDirectory(JsValue path)
    {
        return tryRun("fileAccess.isDirectory", () =>
        {
            if (!getPath("fileAccess.isDirectory", path, out var norm)) return jsFrom(false);
            return jsFrom(Directory.Exists(norm));
        });
    }

    public JsValue createTempFile()
    {
        return tryRun("fileAccess.createTempFile", () =>
        {
            var path = Path.GetTempFileName().Replace("\\", "/");
            return jsFrom(path);
        });
    }
}

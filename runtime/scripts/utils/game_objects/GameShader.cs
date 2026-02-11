using Godot;
using Godot.NativeInterop;
using System;

[GlobalClass]
public partial class GameShader : RefCounted
{
    public string ShaderCode
    {
        get => _shaderCode;
        set
        {
            if (Shader != null)
            {
                _shader.Code = value;
                _shaderMaterial.Shader = _shader;
            }
        }
    }
    public Shader Shader
    {
        get => _shader;
        set
        {
            _shader = value;
            _shader.Code = ShaderCode;
        }
    }

    protected Shader _shader;
    protected ShaderMaterial _shaderMaterial;
    protected string _shaderCode = "";

    public GameShader()
    {
        _shader = new Shader();
        _shaderMaterial = new ShaderMaterial();
        _shaderMaterial.Shader = _shader;
    }

    public void LoadFromFile(string filePath)
    {
        if (string.IsNullOrEmpty(filePath)) return;
        if (ResourceLoader.Exists(filePath))
        {
            Resource res = UtmxResourceLoader.Load(filePath);
            if (res != null && res is Shader shd)
            {
                Shader = shd;
                _shaderMaterial.Shader = shd;
                ShaderCode = filePath;
            }
        }
        else
        {
            FileAccess fileAccess = UtmxResourceLoader.OpenFile(filePath, FileAccess.ModeFlags.Read);
            if (fileAccess != null)
            {
                _shader.SetPathCache(filePath);
                ShaderCode = fileAccess.GetAsText();
            }
        }
    }

    public ShaderMaterial GetShaderMaterial()
    {
        return _shaderMaterial;
    }

    public virtual void SetParameter(string param, object value)
    {
        switch (value)
        {
            case bool boolVal:
                _shaderMaterial.SetShaderParameter(param, boolVal);
                break;
            case int intVal:
                _shaderMaterial.SetShaderParameter(param, intVal);
                break;
            case double:
            case float: 
                _shaderMaterial.SetShaderParameter(param, (float)value);
                break;
            case Color colorVal:
                _shaderMaterial.SetShaderParameter(param, colorVal);
                break;
            case Vector2 vec2Val:
                _shaderMaterial.SetShaderParameter(param, vec2Val);
                break;
            case Vector3 vec3Val:
                _shaderMaterial.SetShaderParameter(param, vec3Val);
                break;
            case Vector4 vec4Val:
                _shaderMaterial.SetShaderParameter(param, vec4Val);
                break;
            case string strVal:
                {
                    Resource res = UtmxResourceLoader.Load(strVal);
                    if (res != null && res is Texture2D texture)
                        _shaderMaterial.SetShaderParameter(param, texture);
                    break;
                }
            case double[]:
            case float[]:
                {
                    float[] arr = (float[])value;
                    if (arr.Length == 4)
                    {
                        Transform2D mat2 = new Transform2D(
                            arr[0], arr[1], 
                            arr[2], arr[3], 
                            0.0F,   0.0F
                            );
                        _shaderMaterial.SetShaderParameter(param, mat2);
                    }
                    else if (arr.Length == 9)
                    {
                        Basis mat3 = new Basis(
                            arr[0], arr[1], arr[2], 
                            arr[3], arr[4], arr[5],
                            arr[6], arr[7], arr[8]
                            );
                        _shaderMaterial.SetShaderParameter(param, mat3);
                    }
                    else if (arr.Length == 16)
                    {
                        Projection mat4 = new Projection(
                            arr[0], arr[1], arr[2], arr[3], 
                            arr[4], arr[5], arr[6], arr[7],
                            arr[8], arr[9], arr[10], arr[11],
                            arr[12], arr[13], arr[14], arr[15]
                            );
                        _shaderMaterial.SetShaderParameter(param, mat4);

                    }
                    else
                    {
                        _shaderMaterial.SetShaderParameter(param, (float[])value);
                    }
                }
                break;
            case object[] objArr:
                {
                    if (objArr.Length == 0) return;
                    if (objArr[0] is string)
                    {
                        Texture[] textures = new Texture[objArr.Length];
                        for (int i = 0; i < textures.Length; i++)
                        {
                            Resource res = UtmxResourceLoader.Load((string)objArr[i]);
                            if (res != null && res is Texture2D texture)
                                textures[i] = texture;
                        }
                        _shaderMaterial.SetShaderParameter(param, textures);
                    }
                    break;
                }
            default:
                UtmxLogger.Error($"{TranslationServer.Translate("Unsupported parameter type:")} {value.GetType().Name}");
                break;
        }
    }
    public object GetParameter(string param)
    {
        return _shaderMaterial.GetShaderParameter(param);
    }
}
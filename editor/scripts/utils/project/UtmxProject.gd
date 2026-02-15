class_name UtmxProject extends Resource;

## 偏好
var favorite : bool = false;

## 项目名称
var project_name : String = "";

## 项目绝对路径
var project_path : String = "";

## 项目icon的相对路径
var icon : String = "";

## 项目icon的纹理对象
var icon_texture : Texture2D = null;

## 项目最后一次打开的时间（使用UTC时间）
var last_open_time : int = 0;

## 项目最后一次打开使用的引擎版本
var engine_version : String = "";

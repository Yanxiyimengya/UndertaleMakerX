import { __UtmxShader } from "__UTMX";

export class UtmxShader extends __UtmxShader {
    constructor(path = null)
    {
        super();
        this.LoadFromFile(path);
    }

    static new(path)
    {
        return new this(path);
    }

	get shaderCode() {
        return this.ShaderCode;
    }
    set shaderCode(value) {
        this.ShaderCode = value;
    }

    loadFrom(filePath)
    {
        this.LoadFromFile(filePath);
    } 
    
    setParameter(param, value)
    {
        this.SetParameter(param, value);
    }
    getParameter(param)
    {
        return this.GetParameter(param);
    }
}
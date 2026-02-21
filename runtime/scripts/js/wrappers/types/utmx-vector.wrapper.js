import { __Vector2, __Vector3, __Vector4 } from "__UTMX";

__Vector2.prototype.copy = function(value) {
    if (value instanceof __Vector2) {
        return new __Vector2(value.x, value.y);
    }
    return new __Vector2(this.x, this.y);
};

__Vector2.prototype.add = function(value) {
    if (value instanceof __Vector2) {
        return new __Vector2(this.x + value.x, this.y + value.y);
    } else {
        return new __Vector2(this.x + value, this.y + value);
    }
};

__Vector2.prototype.subtract = function(value) {
    if (value instanceof __Vector2) {
        return new __Vector2(this.x - value.x, this.y - value.y);
    } else {
        return new __Vector2(this.x - value, this.y - value);
    }
};

__Vector2.prototype.multiply = function(value) {
    if (value instanceof __Vector2) {
        return new __Vector2(this.x * value.x, this.y * value.y);
    } else {
        return new __Vector2(this.x * value, this.y * value);
    }
};

__Vector2.prototype.divide = function(value) {
    if (value instanceof __Vector2) {
        return new __Vector2(this.x / value.x, this.y / value.y);
    } else {
        return new __Vector2(this.x / value, this.y / value);
    }
};

// --- Vector3 实现 ---
__Vector3.prototype.copy = function(value) {
    if (value instanceof __Vector3) {
        this.x = value.x;
        this.y = value.y;
        this.z = value.z;
    }
    return this;
};

__Vector3.prototype.add = function(value) {
    if (value instanceof __Vector3) {
        return new __Vector3(this.x + value.x, this.y + value.y, this.z + value.z);
    } else {
        return new __Vector3(this.x + value, this.y + value, this.z + value);
    }
};

__Vector3.prototype.subtract = function(value) {
    if (value instanceof __Vector3) {
        return new __Vector3(this.x - value.x, this.y - value.y, this.z - value.z);
    } else {
        return new __Vector3(this.x - value, this.y - value, this.z - value);
    }
};

__Vector3.prototype.multiply = function(value) {
    if (value instanceof __Vector3) {
        return new __Vector3(this.x * value.x, this.y * value.y, this.z * value.z);
    } else {
        return new __Vector3(this.x * value, this.y * value, this.z * value);
    }
};

__Vector3.prototype.divide = function(value) {
    if (value instanceof __Vector3) {
        return new __Vector3(this.x / value.x, this.y / value.y, this.z / value.z);
    } else {
        return new __Vector3(this.x / value, this.y / value, this.z / value);
    }
};

// --- Vector4 实现 ---
__Vector4.prototype.copy = function(value) {
    if (value instanceof __Vector4) {
        this.x = value.x;
        this.y = value.y;
        this.z = value.z;
        this.w = value.w;
    }
    return this;
};

__Vector4.prototype.add = function(value) {
    if (value instanceof __Vector4) {
        return new __Vector4(this.x + value.x, this.y + value.y, this.z + value.z, this.w + value.w);
    } else {
        return new __Vector4(this.x + value, this.y + value, this.z + value, this.w + value);
    }
};

__Vector4.prototype.subtract = function(value) {
    if (value instanceof __Vector4) {
        return new __Vector4(this.x - value.x, this.y - value.y, this.z - value.z, this.w - value.w);
    } else {
        return new __Vector4(this.x - value, this.y - value, this.z - value, this.w - value);
    }
};

__Vector4.prototype.multiply = function(value) {
    if (value instanceof __Vector4) {
        return new __Vector4(this.x * value.x, this.y * value.y, this.z * value.z, this.w * value.w);
    } else {
        return new __Vector4(this.x * value, this.y * value, this.z * value, this.w * value);
    }
};

__Vector4.prototype.divide = function(value) {
    if (value instanceof __Vector4) {
        return new __Vector4(this.x / value.x, this.y / value.y, this.z / value.z, this.w / value.w);
    } else {
        return new __Vector4(this.x / value, this.y / value, this.z / value, this.w / value);
    }
};

export {
    __Vector2,
    __Vector3,
    __Vector4,
}

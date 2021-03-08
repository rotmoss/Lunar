layout(std430, binding = 0) buffer projection
{
    mat4 proj;
};
layout(std430, binding = 1) buffer aspectRatio
{
    float aspect;
};

#ifdef VERTEX

layout (location = 0) in vec3 aPos;
layout (location = 2) in vec2 aTexCoord;
layout (location = 3) in float aTexId;

out vec3 Pos;
out vec2 TexCoord;
out int TexId;

void main()
{
    Pos = aPos;
    vec4 pos = vec4(aPos.xyz, 1.0);
    gl_Position = pos * proj;
    TexId = int(aTexId);
    TexCoord = aTexCoord;
}

#endif

#ifdef FRAGMENT

out vec4 FragColor;

in vec3 Pos;
in flat int TexId;
in vec2 TexCoord;
uniform sampler2D textures[32];

void main()
{
    vec4 color = texture(textures[TexId], TexCoord);

    color = vec4(color.r, color.g, color.b, 1.0);
    FragColor = color;
}

#endif
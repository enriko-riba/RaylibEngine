using RaylibEngine.Components;
using System.Numerics;

namespace Deeper
{
	internal class FowSpot : Sprite
	{
		private readonly Shader shdrSpot;
		private int innerLoc;
		private int radiusLoc;
		private int screenHalfLoc;
		
		public FowSpot(Texture texture, Rectangle srcFrame) : base(texture)
		{
			Frame = srcFrame;

			shdrSpot = LoadShader(null, "./Assets/Shaders/vehicle-spot.fs");
			innerLoc = GetShaderLocation(shdrSpot, "inner");
			radiusLoc = GetShaderLocation(shdrSpot, "radius");
			screenHalfLoc = GetShaderLocation(shdrSpot, "screenHalfSize");

			var inner = 48 * 0.2f;
			var radius = 48 * 2f;
			SetShaderValue(shdrSpot, innerLoc, inner, ShaderUniformDataType.SHADER_UNIFORM_FLOAT);
			SetShaderValue(shdrSpot, radiusLoc, radius, ShaderUniformDataType.SHADER_UNIFORM_FLOAT);
			SetShaderValue(shdrSpot, screenHalfLoc, new Vector2(GetScreenWidth()/2, GetScreenHeight()/2), ShaderUniformDataType.SHADER_UNIFORM_VEC2);
		}

		public override void Draw()
		{
			BeginShaderMode(shdrSpot);
			base.Draw();
			EndShaderMode();
		}

		public void OnResize(int width, int height)
		{
			SetShaderValue(shdrSpot, screenHalfLoc, new Vector2(width/2, height/2), ShaderUniformDataType.SHADER_UNIFORM_VEC2);
		}
	}
}

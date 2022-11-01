using RaylibEngine.Components;

namespace Deeper
{
	internal class FowSpot : Sprite
	{
		private readonly Shader shdrSpot;
		private int innerLoc;
		private int radiusLoc;
		private int screenWidthLoc;
		private int screenHeightLoc;

		public FowSpot(Texture texture, Rectangle srcFrame) : base(texture)
		{
			Frame = srcFrame;

			shdrSpot = LoadShader(null, "./Assets/Shaders/vehicle-spot.fs");
			innerLoc = GetShaderLocation(shdrSpot, "inner");
			radiusLoc = GetShaderLocation(shdrSpot, "radius");
			screenWidthLoc = GetShaderLocation(shdrSpot, "screenWidth");
			screenHeightLoc = GetShaderLocation(shdrSpot, "screenHeight");

			var inner = 48 * 0.2f;
			var radius = 48 * 2f;
			SetShaderValue(shdrSpot, innerLoc, inner, ShaderUniformDataType.SHADER_UNIFORM_FLOAT);
			SetShaderValue(shdrSpot, radiusLoc, radius, ShaderUniformDataType.SHADER_UNIFORM_FLOAT);
			SetShaderValue(shdrSpot, screenWidthLoc, (float)GetScreenWidth(), ShaderUniformDataType.SHADER_UNIFORM_FLOAT);
			SetShaderValue(shdrSpot, screenHeightLoc, (float)GetScreenHeight(), ShaderUniformDataType.SHADER_UNIFORM_FLOAT);
		}

		public override void Draw()
		{
			BeginShaderMode(shdrSpot);
			base.Draw();
			EndShaderMode();
		}
	}
}

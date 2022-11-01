using RaylibEngine.Components;
using System.Numerics;

namespace Deeper
{
	internal class VehicleSpotMask : Sprite
	{
		private readonly Shader shdrSpot;
		private int innerLoc;
		private int radiusLoc;
		private int screenHalfLoc;

		public const string NodeName = "SpotMask";

		public VehicleSpotMask(Texture texture, Rectangle srcFrame, float innerRadius, float outerRadius) : base(texture)
		{
			Name = NodeName;
			Frame = srcFrame;

			shdrSpot = LoadShader(null, "./Assets/Shaders/vehicle-spot.fs");
			innerLoc = GetShaderLocation(shdrSpot, "inner");
			radiusLoc = GetShaderLocation(shdrSpot, "radius");
			screenHalfLoc = GetShaderLocation(shdrSpot, "screenHalfSize");

			
			SetShaderValue(shdrSpot, innerLoc, innerRadius, ShaderUniformDataType.SHADER_UNIFORM_FLOAT);
			SetShaderValue(shdrSpot, radiusLoc, outerRadius, ShaderUniformDataType.SHADER_UNIFORM_FLOAT);
			SetShaderValue(shdrSpot, screenHalfLoc, new Vector2(GetScreenWidth()/2, GetScreenHeight()/2), ShaderUniformDataType.SHADER_UNIFORM_VEC2);
		}

		public override void Draw()
		{
			BeginShaderMode(shdrSpot);
			base.Draw();
			EndShaderMode();
		}

		public void UpdateViewport(Vector2 halfSize)
		{
			SetShaderValue(shdrSpot, screenHalfLoc, halfSize, ShaderUniformDataType.SHADER_UNIFORM_VEC2);
		}
	}
}

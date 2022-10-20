namespace RaylibEngine.Core;

public interface IDrawable
{
    void Draw();
	bool Visible { get; set; }
}

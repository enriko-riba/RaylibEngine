namespace RaylibEngine;

public class Helpers
{
    public static float Lerp(float a, float b, float t) => (1f - t) * a + t * b;
}

namespace Box2DTest.Physics2DUtils;

using Box2D.NetStandard.Collision.Shapes;
using Box2D.NetStandard.Dynamics.Bodies;
using Box2D.NetStandard.Dynamics.Fixtures;
using Box2D.NetStandard.Dynamics.World;
using RaylibEngine;
using RaylibEngine.Components;
using System.Numerics;

public static class Box2DHelper
{
    public static Raylib_CsLo.Color ToRaylibColor(this Color color)
    {
        return new Raylib_CsLo.Color()
        {
            r = (byte)Helpers.Lerp(0, 255, color.R),
            g = (byte)Helpers.Lerp(0, 255, color.G),
            b = (byte)Helpers.Lerp(0, 255, color.B),
            a = (byte)Helpers.Lerp(0, 255, color.A),
        };
    }

    public static Body CreateBody(this World world, Sprite sprite, BodyType bodyType, object? userData)
    {
        return world.CreateBody(sprite, bodyType, 0, Vector2.One, userData);
    }

    public static Body CreateBody(this World world, Sprite sprite, BodyType bodyType, float angularVelocity, Vector2 scale, object? userData)
    {
        FixtureDef fd = new()
        {
            friction = 0.1f,
            restitution = 0.01f,
            density = 1f,
            isSensor = false
        };
        return world.CreateBody(sprite, bodyType, new[] { fd }, false, angularVelocity, scale, userData);
    }

    public static Body CreateBody(this World world, Sprite sprite, BodyType bodyType, IEnumerable<FixtureDef> fixtureDefs, bool isBullet, float angularVelocity, Vector2 scale, object? userData)
    {
        BodyDef bd = new()
        {
            type = bodyType,
            position = sprite.Position * scale,
            linearDamping = 0,
            allowSleep = true,
            awake = true,
            bullet = isBullet,
            gravityScale = 1f,
            angle = sprite.Angle * (float)Helpers.DEGREE_2_RADIAN,
            angularVelocity = angularVelocity,
            userData = userData
        };
        var body = world.CreateBody(bd);


        foreach (var fd in fixtureDefs)
        {
            if (fd.shape is null)
            {
                PolygonShape shape = new();
                shape.SetAsBox(scale.X * sprite.Width / 2, scale.Y * sprite.Height / 2);
                fd.shape = shape;
            }
            body.CreateFixture(fd);
        }

        return body;
    }

    public static Body CreateGroundBox(this World world, Vector2 position, int halfWidth, int halfHeight, Vector2 scale, object? userData)
    {
        BodyDef bd = new()
        {
            type = BodyType.Static,
            position = position * scale,
            allowSleep = true,
            awake = true,
            userData = userData
        };
        var body = world.CreateBody(bd);
        FixtureDef fd = new()
        {
            friction = 0.6f,
            restitution = 0.05f,
            isSensor = false
        };
        PolygonShape shape = new();
        shape.SetAsBox(halfWidth * scale.X, halfHeight * scale.Y);
        fd.shape = shape;
        body.CreateFixture(fd);
        return body;
    }
}
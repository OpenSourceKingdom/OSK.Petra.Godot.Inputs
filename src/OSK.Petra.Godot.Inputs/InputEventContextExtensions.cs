using Godot;
using Godot.Collections;
using OSK.Extensions.Petra.Godot;
using OSK.Petra.Inputs.Abstractions.Runtime;
using OSK.Petra.Inputs.Capabilities.Pointer;
using System.Collections.Generic;
using System.Linq;

namespace OSK.Petra.Godot.Inputs;

public static class InputEventContextExtensions
{
    #region 2D

    /// <summary>
    /// Gets a list of game objects the context is able to retrieve from a given pointer location and 2D space
    /// </summary>
    /// <param name="context">The <see cref="IInputEventContext"/> that triggered an action</param>
    /// <param name="space">The 2D space that will be used to handle the pointer collisions</param>
    /// <param name="pointerIndex">The index for the pointer to use from the context's <see cref="PointerFeature"/></param>
    /// <param name="maxResults">The maximum number of objects that are returned from the projection</param>
    /// <returns>A list of <see cref="Node2D"/> colliders that were hit from the given projection</returns>
    public static IEnumerable<Node2D> GetGameObjects(this IInputEventContext context, PhysicsDirectSpaceState2D space,
        int pointerIndex = 0, int maxResults = 1)
    {
        if (!context.TryGetInputFeature<PointerFeature>(out var pointerFeature))
        {
            return [];
        }
        if (pointerFeature.TotalPointers is 0 || pointerIndex < 0 || pointerIndex >= pointerFeature.TotalPointers)
        {
            return [];
        }

        var query = new PhysicsPointQueryParameters2D()
        {
            Position = pointerFeature[pointerIndex].Details.CurrentPosition.ToGodot2(),
            CollideWithAreas = true,
            CollideWithBodies = true
        };

        return space.IntersectPoint(query, maxResults)
            .Select(result => result["collider"].As<Node2D>())
            .Where(node => node is not null);
    }

    #endregion

    #region 3D

    /// <summary>
    /// Attempts to project the pointer location to a given distance from the camera
    /// </summary>
    /// <param name="context">The <see cref="IInputEventContext"/> that triggered an action</param>
    /// <param name="camera">The player camera to project from</param>
    /// <param name="pointerIndex">The index for the pointer to use from the context's <see cref="PointerFeature"/></param>
    /// <param name="projectionDepth">The distance to project the ray</param>
    /// <param name="planeNormalVector">The facing direction of the plane used to check for projection intersection</param>
    /// <returns>Returns the <see cref="Vector3"/> position of the projection of the pointer, if the projection is valid for the camera and pointer.</returns>
    public static Vector3? ProjectPointerToDepth(this IInputEventContext context, Camera3D camera, int pointerIndex = 0,
        float projectionDepth = 1000, Vector3? planeNormalVector = null)
    {
        if (!context.TryGetInputFeature<PointerFeature>(out var pointerFeature))
        {
            return null;
        }
        if (pointerFeature.TotalPointers is 0 || pointerIndex < 0 || pointerIndex >= pointerFeature.TotalPointers)
        {
            return null;
        }

        var vector2 = pointerFeature[pointerIndex].Details.CurrentPosition.ToGodot2();
        var from = camera.ProjectRayOrigin(vector2);
        var dir = camera.ProjectRayNormal(vector2);

        var normal = planeNormalVector ?? Vector3.Up;

        var pointOnPlane = from + (dir * projectionDepth);
        var targetPlane = new Plane(normal, pointOnPlane);

        return targetPlane.IntersectsRay(from, dir);
    }

    /// <summary>
    /// Projects a vector from the specified pointer into the game environment and determines the position where an intersection occurred (i.e. gets the position a potential 'game floor' is located from the pointer)
    /// </summary>
    /// <param name="context">The event context to get the pointer information</param>
    /// <param name="camera">The camera being projected from</param>
    /// <param name="pointerIndex">The desired pointer index</param>
    /// <param name="projectionDepth">How far into the game environment the projected point should be</param>
    /// <returns>The location an intersection with the game environment occurred</returns>
    public static Vector3? FindIntersectionFromPointer(this IInputEventContext context, Camera3D camera, int pointerIndex = 0, float projectionDepth = 1000)
    {
        if (!context.TryGetInputFeature<PointerFeature>(out var pointerFeature))
        {
            return null;
        }
        if (pointerFeature.TotalPointers is 0 || pointerIndex < 0 || pointerIndex >= pointerFeature.TotalPointers)
        {
            return null;
        }

        var vector2 = pointerFeature[pointerIndex].Details.CurrentPosition.ToGodot2();
        var from = camera.ProjectRayOrigin(vector2);
        var dir = camera.ProjectRayNormal(vector2);

        var query = PhysicsRayQueryParameters3D.Create(
            from,
            from + dir * projectionDepth);

        query.CollideWithAreas = true;

        var result = camera.GetWorld3D().DirectSpaceState.IntersectRay(query);
        return result.Count is 0
            ? null
            : result["position"].AsVector3();
    }

    /// <summary>
    /// Gets a list of game objects the context is able to retrieve from a given pointer location and player camera in 3D space
    /// </summary>
    /// <param name="context">The <see cref="IInputEventContext"/> that triggered an action</param>
    /// <param name="camera">The player camera to project from</param>
    /// <param name="pointerIndex">The index for the pointer to use from the context's <see cref="PointerFeature"/></param>
    /// <param name="projectionDepth">The distance to project the ray</param>
    /// <param name="maxResults">The maximum number of objects that are returned from the projection</param>
    /// <returns>A list of <see cref="Node3D"/> colliders that were hit from the given projection</returns>
    public static IEnumerable<Node3D> GetGameObjects(this IInputEventContext context, Camera3D camera,
        int pointerIndex = 0, int projectionDepth = 1000, int maxResults = 1)
    {
        if (!context.TryGetInputFeature<PointerFeature>(out var pointerFeature))
        {
            yield break;
        }
        if (pointerFeature.TotalPointers is 0 || pointerIndex < 0 || pointerIndex >= pointerFeature.TotalPointers)
        {
            yield break;
        }

        var vector2 = pointerFeature[pointerIndex].Details.CurrentPosition.ToGodot2();
        var from = camera.ProjectRayOrigin(vector2);
        var to = from + camera.ProjectRayNormal(vector2) * projectionDepth;

        var query = PhysicsRayQueryParameters3D.Create(from, to);

        query.CollideWithAreas = true;
        var space = camera.GetWorld3D().DirectSpaceState;

        var exclude = new Array<Rid>();

        for (int i = 0; i < maxResults; i++)
        {
            query.Exclude = exclude;
            var result = space.IntersectRay(query);

            if (result.Count == 0)
            {
                break;
            }

            if (result["collider"].As<Node3D>() is Node3D target)
            {
                yield return target;
                // Add the hit object's RID to exclusion so we don't hit it again
                exclude.Add((Rid)result["rid"]);
            }
            else
            {
                // Safety break if we hit something that isn't a Node3D
                break;
            }
        }
    }

    #endregion
}

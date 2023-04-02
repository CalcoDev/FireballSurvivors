using System;
using System.Collections.Generic;
using System.Linq;
using Game.GameObjects;
using Game.GameObjects.Eneimes;
using Game.Logic;
using Godot;
using GodotUtilities;

namespace Game.Directors;

public partial class EnemyDirector : Node
{
    public static EnemyDirector Instance { get; private set; }

    [Export] public float BaseSpawnTime;
    [Export] public float SpawnRadius = 350f;

    [Export] private LootTable _enemiesTable;
    [Export] private ArenaDirector _arenaDirector;

    [Node] private Timer _timer;

    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
        {
            if (Instance != null)
                GD.Print("Enemy Director already exists! OVwerwriting...");
            Instance = this;

            this.WireNodes();
        }
    }

    public override void _Ready()
    {
        _timer.WaitTime = BaseSpawnTime;
        _timer.Connect(Timer.SignalName.Timeout, new Callable(this, MethodName.OnTimerTimeout));

        _arenaDirector.Connect(ArenaDirector.SignalName.OnDifficultyChanged,
            new Callable(this, MethodName.OnDifficultyChanged));
    }

    public IList<Node2D> GetEnemiesInRange(Vector2 pos, float range)
    {
        var enemies = GetTree().GetNodesInGroup("enemy");
        if (enemies.Count == 0)
            return null;

        var enemiesInRange = enemies.Where(e =>
            ((Node2D)e).GlobalPosition.DistanceSquaredTo(pos) <= range * range).ToList();

        if (enemiesInRange.Count == 0)
            return null;

        enemiesInRange.Sort((a, b) =>
            ((Node2D)a).GlobalPosition.DistanceSquaredTo(pos)
                .CompareTo(((Node2D)b).GlobalPosition.DistanceSquaredTo(pos)));

        return enemiesInRange.ConvertAll(e => (Node2D)e);
    }

    private Vector2 GetRandomPosition()
    {
        if (Player.Instance?.IsInsideTree() == false)
            return Vector2.Zero;

        var playerPos = Player.Instance.GlobalPosition;
        var randDir = Vector2.Right.Rotated((float)GD.RandRange(0, Mathf.Tau)) * SpawnRadius;

        var parameters = new PhysicsRayQueryParameters2D()
        {
            CollisionMask = 1 << 0,
            From = playerPos,
            To = playerPos + randDir,
        };
        var result = GetTree().Root.World2D.DirectSpaceState.IntersectRay(parameters);
        if (result.Count == 0)
            return playerPos + randDir;

        while (result.Count > 0)
        {
            randDir = randDir.Rotated(Mathf.Pi / 2f);
            parameters.To = playerPos + randDir;
            result = GetTree().Root.World2D.DirectSpaceState.IntersectRay(parameters);
        }

        return playerPos + randDir;
    }

    private void OnTimerTimeout()
    {
        _timer.Start();

        var randPos = GetRandomPosition();

        var instance = _enemiesTable.PickRandom<PackedScene>().Instantiate<Enemy>();

        // TODO(calco): Probably should cache this somehow.
        GetTree().GetFirstNodeInGroup("ysort_layer")?.CallDeferred(Node.MethodName.AddChild, instance);

        instance.GlobalPosition = randPos;
    }

    private void OnDifficultyChanged(float difficulty)
    {
        float timeOff = 0.1f / (60f / 5f) * difficulty;

        _timer.WaitTime = Mathf.Min(BaseSpawnTime - timeOff, 0.25f);
    }
}
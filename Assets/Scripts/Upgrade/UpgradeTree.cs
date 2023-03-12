using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Node
{
    public string title;
    public string description;
    public Sprite sprite;
    public double cost;
    public List<int> prerequisites;
    public List<int> exclusion;
    public bool bought;
    public int color;

    public Vector2 position;

    public Node(string title, string description, Sprite sprite, double cost, List<int> prerequisites, List<int> exclusion, bool bought, Vector2 position)
    {
        this.title = title;
        this.description = description;
        this.sprite = sprite;
        this.prerequisites = prerequisites;
        this.exclusion = exclusion;
        this.bought = bought;
        this.position = position;
    }

    public Node(Node other)
    {
        this.title = other.title;
        this.description = other.description;
        this.sprite = other.sprite;

        this.prerequisites = new List<int>();
        this.exclusion = new List<int>();

        foreach (var upgrade in other.prerequisites)
            this.prerequisites.Add(upgrade);

        foreach (var upgrade in other.exclusion)
            this.exclusion.Add(upgrade);

        this.position = other.position;

        this.bought = false;
    }
}

[CreateAssetMenu(menuName = "Upgrades/Tree")]
[System.Serializable]
public class UpgradeTree : ScriptableObject
{
    [SerializeField]
    public List<Node> tree;

    public int size { get { return tree.Count; } }

    public UpgradeTree Init(UpgradeTree other)
    {
        tree = new List<Node>();

        if (other == null || other.tree == null)
            return this;

        foreach (var node in other.tree)
        {
            tree.Add(new Node(node));
        }

        return this;
    }

    public bool AddNode(string title, Vector2 pos)
    {
        int idx = IndexOf(title);

        if (idx != -1)
            return false;

        tree.Add(new Node(title, "", null, 0.0, new List<int>(), new List<int>(), false, pos));
        return true;
    }

    public void DeleteNode(int idx)
    {
        if (idx is int i)
        {
            foreach (Node n in tree)
            {
                n.prerequisites.RemoveAll(u => u == i);
                n.exclusion.RemoveAll(u => u == i);
            }

            tree.RemoveAt(i);
        }
    }

    public void DeleteNode(Node node) => DeleteNode(IndexOf(node));

    public bool HasCycle(int? query, int subject, bool checkExclusion = false)
    {
        if (!query.HasValue)
            return false;

        var edges = checkExclusion ? tree[query.Value].exclusion : tree[query.Value].prerequisites;

        foreach (var v in edges)
        {
            if (v == subject)
                return true;

            if (HasCycle(v, subject, checkExclusion))
                return true;
        }

        return false;
    }

    public bool HasExclusiveAncestors(int begin, int end)
    {
        var ancestors = GetAncestors(begin, true);
        ancestors.UnionWith(GetAncestors(end, true));
        ancestors.Add(begin);

        foreach (var ancestor in ancestors)
        {
            foreach (var excluded in tree[ancestor].exclusion)
            {
                if (ancestors.Contains(excluded))
                    return true;
            }
        }

        return false;
    }

    public bool Buyable(int node)
    {
        // FIXME: This is slow, IndexOf is O(n) and must check O(m) ancestors -> O(nm) ~ O(n^2) in worst case.
        var ancestors = GetAncestors(node, true);

        foreach (var a in ancestors)
        {
            Node ancestor = tree[a];

            if (!ancestor.bought)
                return false;
        }

        return true;
    }

    public List<Node> GetBuyable()
    {
        var buyable = new List<Node>();
        foreach ((Node node, int i) in tree.WithIndex())
        {
            if (!node.bought && Buyable(i))
                buyable.Add(node);
        }

        return buyable;
    }

    public bool Connectable(int begin, int end)
    {
        if (begin == end)
            return false;

        bool invalid = HasCycle(end, begin) || HasCycle(begin, end) || HasExclusiveAncestors(begin, end);

        return !invalid;
    }

    public HashSet<int> GetAncestors(int node, bool includeImmediateAncestors)
    {
        var ancestors = (includeImmediateAncestors) ? new HashSet<int>(tree[node].prerequisites) : new HashSet<int>();

        foreach (int u in tree[node].prerequisites)
            ancestors.UnionWith(GetAncestors(u, true));

        return ancestors;
    }

    public void RemoveCycles(int idx)
    {
        var ancestors = GetAncestors(idx, false);

        tree[idx].prerequisites.RemoveAll(item => ancestors.Contains(item));
    }

    public int IndexOf(string upgrade)
    {
        for (int i = 0; i < tree.Count; i++)
            if (tree[i].title == upgrade)
                return i;

        return -1;
    }

    public int IndexOf(Node node)
    {
        return tree.IndexOf(node);
    }

    public Node this[int key]
    {
        get => tree[key];
        set => tree[key] = value;
    }
}

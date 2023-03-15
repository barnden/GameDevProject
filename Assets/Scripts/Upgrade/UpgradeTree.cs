using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Tree")]
[System.Serializable]
public class UpgradeTree : ScriptableObject
{
    [SerializeField]
    public List<UpgradeNode> tree;

    public int size { get { return tree.Count; } }

    public UpgradeTree Init(UpgradeTree other)
    {
        tree = new List<UpgradeNode>();

        if (other == null || other.tree == null)
            return this;

        foreach (var otherNode in other.tree)
        {
            tree.Add(new UpgradeNode(otherNode));
        }

        return this;
    }

    public bool AddNode(string title, Vector2 pos)
    {
        int idx = IndexOf(title);

        if (idx != -1)
            return false;

        tree.Add(new UpgradeNode(title, "", null, 0.0, new List<int>(), new List<int>(), false, pos));
        return true;
    }

    public void DeleteNode(int idx)
    {
        if (idx is int i)
        {
            foreach (UpgradeNode n in tree)
            {
                n.prerequisites.RemoveAll(u => u == i);
                n.exclusion.RemoveAll(u => u == i);
            }

            tree.RemoveAt(i);
        }
    }

    public void DeleteNode(UpgradeNode node) => DeleteNode(IndexOf(node));

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
            UpgradeNode ancestor = tree[a];

            if (!ancestor.bought)
                return false;
        }

        return true;
    }

    public List<int> GetBuyable()
    {
        var buyable = new List<int>();
        foreach ((UpgradeNode node, int i) in tree.WithIndex())
        {
            if (!node.bought && Buyable(i))
                buyable.Add(i);
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

    public int GetParentSprite(int node, HashSet<int> ancestors = null)
    {
        if (ancestors == null)
            ancestors = GetAncestors(node, true);

        int zindexMax = int.MinValue;
        int parentSprite = -1;

        foreach (var u in ancestors)
        {
            UpgradeNode ancestor = tree[u];
            var zindex = ancestor.inheritSprite ? int.MinValue : ancestor.zindex;

            if (zindexMax < zindex)
            {
                zindexMax = zindex;
                parentSprite = u;
            }
        }

        return parentSprite;
    }

    public int GetParentSprite(UpgradeNode node, HashSet<int> ancestors = null) => GetParentSprite(IndexOf(node), ancestors);

    public Sprite GetEffectiveSprite(int node)
    {
        int parent = -1;

        if (tree[node].inheritSprite)
            parent = GetParentSprite(node);

        return (parent == -1) ? tree[node].sprite : tree[parent].sprite;
    }

    public Sprite GetEffectiveSprite(UpgradeNode node) => GetEffectiveSprite(IndexOf(node));

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

    public int IndexOf(UpgradeNode node)
    {
        return tree.IndexOf(node);
    }

    public UpgradeNode this[int key]
    {
        get => tree[key];
        set => tree[key] = value;
    }
}

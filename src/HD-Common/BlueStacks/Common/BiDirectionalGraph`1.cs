// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.BiDirectionalGraph`1
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BlueStacks.Common
{
  public class BiDirectionalGraph<T>
  {
    public ObservableCollection<BiDirectionalVertex<T>> Vertices { get; }

    public BiDirectionalGraph(
      ObservableCollection<BiDirectionalVertex<T>> initialNodes = null)
    {
      this.Vertices = initialNodes ?? new ObservableCollection<BiDirectionalVertex<T>>();
    }

    public void AddVertex(BiDirectionalVertex<T> vertex)
    {
      if (vertex == null || this.Vertices.Contains(vertex))
        return;
      this.Vertices.Add(vertex);
    }

    public void AddParentChild(BiDirectionalVertex<T> parent, BiDirectionalVertex<T> child)
    {
      if (parent == null || child == null)
        return;
      this.AddVertex(parent);
      this.AddVertex(child);
      this.AddParentChildRelation(parent, child);
    }

    public void RemoveVertex(BiDirectionalVertex<T> vertex)
    {
      if (vertex == null || !this.Vertices.Contains(vertex))
        return;
      this.DeLinkMacro(vertex);
      this.Vertices.Remove(vertex);
    }

    public void DeLinkMacroChild(BiDirectionalVertex<T> recording)
    {
      if (recording == null)
        return;
      foreach (BiDirectionalVertex<T> child in recording.Childs)
        child.RemoveParent(recording);
      recording.Childs.Clear();
    }

    public void DeLinkMacroParent(BiDirectionalVertex<T> recording)
    {
      if (recording == null)
        return;
      foreach (BiDirectionalVertex<T> parent in recording.Parents)
        parent.RemoveChild(recording);
      recording.Parents.Clear();
    }

    public void DeLinkMacro(BiDirectionalVertex<T> recording)
    {
      this.DeLinkMacroChild(recording);
      this.DeLinkMacroParent(recording);
    }

    private void AddParentChildRelation(BiDirectionalVertex<T> parent, BiDirectionalVertex<T> child)
    {
      if (parent == null || child == null)
        return;
      if (!child.Parents.Contains(parent))
        child.AddParent(parent);
      if (parent.Childs.Contains(child))
        return;
      parent.AddChild(child);
    }

    private bool ChildExist(BiDirectionalVertex<T> root, BiDirectionalVertex<T> searchVertex)
    {
      if (root.IsVisited)
        return false;
      root.IsVisited = true;
      return root.Equals((object) searchVertex) || root.Childs.Any<BiDirectionalVertex<T>>((Func<BiDirectionalVertex<T>, bool>) (child => this.ChildExist(child, searchVertex)));
    }

    public bool DoesParentExist(BiDirectionalVertex<T> root, BiDirectionalVertex<T> searchVertex)
    {
      if (root == null)
        return false;
      this.UnVisitAllVertices();
      return this.ParentExist(root, searchVertex);
    }

    private bool ParentExist(BiDirectionalVertex<T> root, BiDirectionalVertex<T> searchVertex)
    {
      if (root.IsVisited)
        return false;
      root.IsVisited = true;
      return root.Equals((object) searchVertex) || root.Parents.Any<BiDirectionalVertex<T>>((Func<BiDirectionalVertex<T>, bool>) (parent => this.ParentExist(parent, searchVertex)));
    }

    private void UnVisitAllVertices()
    {
      foreach (BiDirectionalVertex<T> vertex in (Collection<BiDirectionalVertex<T>>) this.Vertices)
        vertex.IsVisited = false;
    }

    public List<BiDirectionalVertex<T>> GetAllChilds(
      BiDirectionalVertex<T> vertex)
    {
      List<BiDirectionalVertex<T>> dependents = new List<BiDirectionalVertex<T>>();
      if (vertex != null)
        GetChilds(vertex);
      return dependents;

      void GetChilds(BiDirectionalVertex<T> node)
      {
        foreach (BiDirectionalVertex<T> child in node.Childs)
        {
          if (!dependents.Contains(child))
          {
            dependents.Add(child);
            if (child.Childs.Count > 0)
              GetChilds(child);
          }
        }
      }
    }

    public List<MacroRecording> GetAllLeaves(MacroRecording macro)
    {
      List<MacroRecording> leaves = new List<MacroRecording>();
      if (macro != null)
        IterateForLeaves(macro);
      return leaves;

      void IterateForLeaves(MacroRecording recording)
      {
        foreach (MacroRecording child in recording.Childs)
        {
          if (child.Childs.Count == 0 && !leaves.Contains(child))
            leaves.Add(child);
          else
            IterateForLeaves(child);
        }
      }
    }
  }
}

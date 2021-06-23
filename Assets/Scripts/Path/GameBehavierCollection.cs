using System.Collections.Generic;

[System.Serializable]
public class GameBehavierCollection
{
  List<GameBehavier> behaviers = new List<GameBehavier>();

  public bool IsEmpty => behaviers.Count == 0;

  public void Add(GameBehavier behavier)
  {
    behaviers.Add(behavier);
  }

  public void GameUpdate()
  {
    for (int i = 0; i < behaviers.Count; i++)
    {
      if (!behaviers[i].GameUpdate())
      {
        int lastIndex = behaviers.Count - 1;
        behaviers[i] = behaviers[lastIndex];
        behaviers.RemoveAt(lastIndex);
        --i;
      }
    }
  }

  public void Clear()
  {
    for (int i = 0; i < behaviers.Count; i++)
    {
      behaviers[i].Recycle();
    }
    behaviers.Clear();
  }
}

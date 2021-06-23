using UnityEngine;

public abstract class GameBehavier : MonoBehaviour
{
  public virtual bool GameUpdate() => true;

  public abstract void Recycle();
}

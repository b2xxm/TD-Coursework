using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // make proxy later
    public EnemyType type;
    public int health;
    public float speed;

    public void SetData(EnemyData data)
    {
        // <note> check if is not traversing
        type = data.type;
        health = data.health;
        speed = data.speed;
    }

    public IEnumerator Traverse(List<Tile> path, Action finishCallback)
    {
        for (int index = 0; index < path.Count; index++) {
            Tile tile = path[index];

            if (index == 0) {
                Vector3 tilePosition = tile.transform.position;
                transform.position = new(tilePosition.x, tilePosition.y, -1);

                continue;
            }

            yield return StartCoroutine(MoveTo(tile));
        }

        finishCallback();
    }

    private IEnumerator MoveTo(Tile tile)
    {
        Vector3 start = transform.position;
        Vector3 end = tile.transform.position;

        float totalTime = 1 / speed;
        float elapsed = 0;

        while (true) {
            elapsed = Mathf.Min(elapsed + Time.deltaTime, totalTime);

            float alpha = elapsed / totalTime;

            Vector3 position = Vector3.Lerp(start, end, alpha);
            transform.position = new(position.x, position.y, -1);

            if (alpha == 1) {
                break;
            }

            yield return null;
        }
    }
}

public enum EnemyType
{
    Normal,
    Fast,
    Slow
}

/* TO-DO

 * Enemy behaviour
   - health
   - base damaging

*/
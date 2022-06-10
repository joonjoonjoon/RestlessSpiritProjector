using UnityEngine;
using System.Collections;

public static class Physics2DExtension
{
    public static RaycastHit2D LinecastFrontmost(Vector3 clickPosition, int layerMask)
    {
        RaycastHit2D result = new RaycastHit2D();

        SpriteRenderer spriteRenderer;

        // Retrieve all raycast hits from the click position and store them in an array called "hits".
        RaycastHit2D[] hits = Physics2D.LinecastAll(clickPosition, clickPosition, layerMask);

        // If the raycast hits something...
        if (hits.Length != 0)
        {
            // A variable that will store the frontmost sorting layer that contains an object that has been clicked on as an int.
            int topSortingLayer = 0;
            // An array that stores the IDs of all the sorting layers that contain a sprite in the path of the linecast.
            int[] sortingLayerIDArray = new int[hits.Length];
            // An array that stores the sorting orders of each sprite that has been hit by the linecast
            int[] sortingOrderArray = new int[hits.Length];
            // An array that stores the sorting order number of the frontmost sprite that has been clicked.
            int topSortingOrder = 0;
            // A variable that will store the index in the sortingOrderArray where topSortingOrder is. This index used with the hits array will give us our frontmost clicked sprite.
            int indexOfTopSortingOrder = 0;

            // Loop through the array of raycast hits...
            for (var i = 0; i < hits.Length; i++)
            {
                // Get the SpriteRenderer from each game object under the click.
                spriteRenderer = hits[i].collider.gameObject.GetComponentInChildren<SpriteRenderer>();

                if (spriteRenderer == null)
                {
                    // Access the sortingLayerID through the SpriteRenderer and store it in the sortingLayerIDArray.
                    sortingLayerIDArray[i] = 0;

                    // Access the sortingOrder through the SpriteRenderer and store it in the sortingOrderArray.
                    sortingOrderArray[i] = -1000;
                }
                else
                {
                    // Access the sortingLayerID through the SpriteRenderer and store it in the sortingLayerIDArray.
                    sortingLayerIDArray[i] = spriteRenderer.sortingLayerID;

                    // Access the sortingOrder through the SpriteRenderer and store it in the sortingOrderArray.
                    sortingOrderArray[i] = spriteRenderer.sortingOrder;
                }
            }

            // Loop through the array of sprite sorting layer IDs...
            for (int j = 0; j < sortingLayerIDArray.Length; j++)
            {
                // If the sortingLayerID is higher that the topSortingLayer...
                if (sortingLayerIDArray[j] >= topSortingLayer)
                {
                    topSortingLayer = sortingLayerIDArray[j];
                }
            }

            // Loop through the array of sprite sorting orders...
            for (int k = 0; k < sortingOrderArray.Length; k++)
            {
                // If the sorting order of the sprite is higher than topSortingOrder AND the sprite is on the top sorting layer...
                if (sortingOrderArray[k] >= topSortingOrder && sortingLayerIDArray[k] == topSortingLayer)
                {
                    topSortingOrder = sortingOrderArray[k];
                    indexOfTopSortingOrder = k;
                }
                else
                {
                    // Do nothing and continue loop.
                }
            }

            // The indexOfTopSortingOrder will also be the index of the frontmost raycast hit in the array "hits".
            result = hits[indexOfTopSortingOrder];
        }
        return result;
    }
}

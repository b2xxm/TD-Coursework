using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path
{
    private const double maxSeed = 1e9;

    private System.Random random; // System.Random is better than Random - allows for better random control
    private Tile endTile;

    private readonly Field grid;
    private readonly List<Tile> blacklist; // Caches each visited tile that will always be invalid

    public int Seed { get; private set; } // Can only be set within the class
    public List<Tile> Pathway { get; }

    // Called when a new object is instantiated
    public Path(Field grid)
    {
        this.grid = grid;

        Pathway = new();
        blacklist = new();
    }

    // Generates a new pseudorandom pathway. Optional seed argument
    public void Generate(int? givenSeed)
    {
        Reset();
        SetSeed(givenSeed);

        int rows = grid.Rows;
        int columns = grid.Columns;

        // Next's maximum argument is exclusive: range = 0 -> 4
        Address startAddress = new(random.Next(rows), 0);
        Address endAddress = new(random.Next(rows), columns - 1); // Addressing starts at 0 -> dimensions.y - 1

        Tile start = grid.FindTileByAddress(startAddress);
        Tile end = grid.FindTileByAddress(endAddress);

        endTile = end;

        // Blacklisting first and last columns, for preference
        // Makes it seem that enemies come out from the edge
        for (int column = 0; column < columns; column += columns - 1) { // Increment by dimensions - 1 to get first and last column
            for (int row = 0; row < rows; row++) {
                Address address = new(row, column);

                // Make sure that the path can actually be created
                if (address.Equals(endAddress))
                    continue;

                Tile tile = grid.FindTileByAddress(address);
                blacklist.Add(tile);
            }
        }

        CreatePath(start);
        Visualize();
    }

    // Clears the current pathway, and resets the blacklist; for the next path generation
    private void Reset()
    {
        Pathway.Clear();
        blacklist.Clear();
    }

    // Returns the next tile (or null) to traverse to, relative to the given tile
    private Tile FindNextTile(Tile tile)
    {
        // Adjacent tiles are next to given tile
        List<Tile> adjacent = tile.GetAdjacent();
        List<Tile> candidates = new();

        // Checks validity of each adjacent tile
        foreach (Tile adjacentTile in adjacent) {
            if (blacklist.Contains(adjacentTile)) {
                continue;
            }

            // Neighbouring tiles are next to adjacent tiles
            List<Tile> neighbours = adjacentTile.GetAdjacent();
            int neighboursInPath = 0;

            foreach (Tile neighbour in neighbours) {
                if (!Pathway.Contains(neighbour)) {
                    continue;
                }

                neighboursInPath += 1;

                // More than 1 tile occurring in pathway means path adjacency
                if (neighboursInPath > 1) {
                    break;
                }
            }

            if (neighboursInPath > 1) {
                continue;
            }

            // Tile is valid, and is appended to candidate list
            candidates.Add(adjacentTile);
        }

        // No candidates, all adjacent tiles are blacklisted or invalid
        if (candidates.Count == 0) {
            return null;
        }

        // Chooses a random candidate
        return candidates[random.Next(candidates.Count)];
    }

    // Recursively backtracks along the pathway until a tile with valid candidates is found
    private Tile Backtrack()
    {
        Tile lastAddedTile = Pathway[Pathway.Count - 1];
        Tile nextTile = FindNextTile(lastAddedTile);

        if (nextTile != null) {
            return nextTile; // Tile to traverse to is finally returned, if found
        }

        // Stack strucutre, pop lasts tile
        Pathway.RemoveAt(Pathway.Count - 1);

        return Backtrack(); // Returns returned tile to traverse to
    }

    private void CreatePath(Tile tile)
    {
        // Adds given tile to pathway, and blacklist because:
            // Paths won't be able to intersect
            // When tile is popped, it can never be a candidate (minor optimisation: tile won't need to be checked again)
        Pathway.Add(tile);
        blacklist.Add(tile);

        // Found tile is the determined end tile
        if (tile == endTile) {
            return;
        }

        // Gets the next tile, and adds it to the pathway, recursively
        Tile nextTile = Backtrack();
        CreatePath(nextTile);
    }

    // Renders the pathway to a different colour
    private void Visualize()
    {
        foreach (Tile tile in Pathway) {
            Renderer renderer = tile.GetComponent<Renderer>();
            renderer.material.color = Color.HSVToRGB(0f, 0f, 0.4f);
        }
    }

    // Creates a new random object, determined by given seed
    private void SetSeed(int? newSeed)
    {
        if (!newSeed.HasValue || newSeed <= 0 || newSeed >= maxSeed) {
            Seed = Random.Range(1, (int) maxSeed);
        } else {
            Seed = (int) newSeed;
        }

        random = new(Seed);
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public const float size = 1; // Arbitrary size value

    private Field grid;

    private readonly List<Vector2Int> directions = new()
    {
        new(-1, 0), new(1, 0), new(0, 1), new(0, -1) // Direction vectors, to find the relative adjacent tiles
    };

    [SerializeField] private GameObject selectHighlight;
    [SerializeField] private GameObject hoverHighlight;

    public SpriteRenderer spriteRenderer;
    public Address Address { get; private set; }
    public Tower Occupier { get; private set; }

    public void Initialize(Field grid, Address address)
    {
        this.grid = grid;
        Address = address;
    }

    public List<Tile> GetAdjacent()
    {
        List<Tile> adjacent = new();

        int currentRow = Address.Row;
        int currentColumn = Address.Column;

        // Gets each tile in each direction, relative to the current tile
        foreach (Vector2Int direction in directions) {
            int row = currentRow + direction.x;
            int column = currentColumn + direction.y;

            // Creates a new address using new values, and tries finding if the tile exists
            Address newAddress = new(row, column);
            Tile tile = grid.FindTileByAddress(newAddress);

            if (tile == null) {
                continue;
            }

            // Adds to list if it exists
            adjacent.Add(tile);
        }

        return adjacent;
    }

    public void Highlight(bool enabled)
    {
        selectHighlight.SetActive(enabled);
    }

    public void PlaceTower(Tower tower)
    {
        Occupier = tower;

        tower.transform.parent = transform;
        tower.transform.localPosition = new(0, 0, -5);
    }

    public void OnMouseDown()
    {
        Path pathObject = grid.PathObject;

        if (pathObject.Pathway.Contains(this))
            return;

        grid.SelectTile(this);
    }

    public void OnMouseEnter()
    {
        Path pathObject = grid.PathObject;

        if (pathObject.Pathway.Contains(this))
            return;

        hoverHighlight.SetActive(true);
    }

    public void OnMouseExit()
    {
        Path pathObject = grid.PathObject;

        if (pathObject.Pathway.Contains(this))
            return;

        hoverHighlight.SetActive(false);
    }
}

// Vector2Int's can be confusing to store rows and columns (x and y)
// This class makes it readable and intuitive
public class Address
{
    public int Row { get; }
    public int Column { get; }

    public Address(int row, int column)
    {
        Row = row;
        Column = column;
    }

    // Custom class allows for added functionality
    public override string ToString()
    {
        return $"({Row},{Column})";
    }

    // Allows for equality checking, despite being different instances with the same values
    public override bool Equals(object other)
    {
        // Attempts to cast given object to type Address. `is not` check fails if cast is invalid, otherwise otherAddress holds the casted value
        // Used instead of explicit typecasting ( (Type)value ) as an exception can be thrown
        if (other is not Address otherAddress)
            return false;

        // Checks that the row and columns of the two Addresses match
        return Row == otherAddress.Row && Column == otherAddress.Column;
    }

    // Allows for the object to be used for sets and gets in hash-based collections
    public override int GetHashCode()
    {
        // Combines two integers, and creates a "unique" hashcode to represent them
        return HashCode.Combine(Row, Column);
    }
}
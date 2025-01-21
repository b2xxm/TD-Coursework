using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public const float size = 1;

    private Field grid;
    private Address address;

    public void Initialize(Field grid, Address address)
    {
        this.grid = grid;
        this.address = address;
    }

    public List<Tile> GetAdjacent()
    {
        List<Tile> adjacent = new();

        int currentRow = address.Row;
        int currentColumn = address.Column;

        for (int row = currentRow - 1; row <= currentRow + 1; row++) {
            for (int column = currentColumn - 1; column <= currentColumn + 1; column++) {
                if (row == currentRow && column == currentColumn) {
                    continue;
                }

                if (row != currentRow && column != currentColumn) {
                    continue;
                }

                Address newAddress = new(row, column);
                Tile tile = grid.FindTileByAddress(newAddress);

                if (tile == null) {
                    continue;
                }

                adjacent.Add(tile);
            }
        }

        return adjacent;
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

// With this new implementation of the Address class, Field.FindTileByAddress breaks, and equality of addresses doesn't work
// Despite the row and columns of the two Address objects being similar, they are difference objects
// Because of this, the method always returns null, and therefore the code which uses this method fails
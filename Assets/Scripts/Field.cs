using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{
    private Dictionary<Address, Tile> tiles;
    private Path path;

    // Values are set within the inspector
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject container;
    [SerializeField] private float padding;
    [SerializeField] private int rows;
    [SerializeField] private int columns;

    public Spawner spawner;
    public Base endBase;

    public int Rows { get { return rows; } }
    public int Columns { get { return columns; } }

    // Awake is called when the game starts running
    private void Awake()
    {
        tiles = new();
        path = new(this);

        // Generates a new grid
        GenerateGrid();

        // <temp> shouldn't be called on start-up
        NewPathway(null);
        StartCoroutine(spawner.RunSchedule(path.Pathway));
        // </temp>
    }

    // Creates a grid of tile game objects
    public void GenerateGrid()
    {
        // Nested for loop, for creating rows and columns
        for (int column = 0; column < columns; column++) {
            for (int row = 0; row < rows; row++) {
                // Creates an address based off of the row and column, converts it to a world position
                Address address = new(row, column);
                Vector3 position = AddressToPosition(address);

                // Clones a new tile game object, from a prefabrication of another tile, sets the parent to the container object
                // Repositions the new tile to the calculated position
                GameObject tileInstance = Instantiate(tilePrefab, container.transform);
                tileInstance.transform.localPosition = position;
                tileInstance.name = address.ToString();

                // Gets the Tile component from the tile game object
                // Initializes the Tile
                Tile tile = tileInstance.GetComponent<Tile>();
                tile.Initialize(this, address);

                // Appens the Tile component of the tile game object to a dictionary, in which it maps the address to the component
                tiles.Add(address, tile);
            }
        }
    }

    // Converts the given address to a world position
    public Vector3 AddressToPosition(Address address)
    {
        // Calculations to determine the position of the address, and returns the position
        float size = Tile.size;
        float gridWidth = columns * (size + padding) - padding;
        float gridHeight = rows * (size + padding) - padding;
        float xOffset = (gridWidth - size) / 2;
        float yOffset = (gridHeight - size) / 2;
        float x = address.Column * (size + padding) - xOffset;
        float y = address.Row * (size + padding) - yOffset;

        return new(x, y);
    }

    // Only calls the Generate method on the path, which generates a new path // ignore iteration 1
    public void NewPathway(int? seed)
    {
        path.Generate(seed);
    }

    // Finds, and returns the tile component via address if it is found, else null is returned
    public Tile FindTileByAddress(Address address)
    {
        bool success = tiles.TryGetValue(address, out Tile tile);

        // Ternary statement shortens it to a one-liner, same logic to using if statements
        return success ? tile : null;
    }
}
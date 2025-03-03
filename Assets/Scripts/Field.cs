using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Field : MonoBehaviour
{
    private Dictionary<Address, Tile> tiles;

    // Values are set within the inspector
    [SerializeField] private Spawner spawner;
    [SerializeField] private Base endBase;
    [SerializeField] private TowerManager towerManager;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject container;
    [SerializeField] private TMP_InputField seedInput;
    [SerializeField] private GameObject buttons;
    [SerializeField] private GameObject towerMenu;
    [SerializeField] private EndMenu endMenu;
    [SerializeField] private float padding;
    [SerializeField] private int rows;
    [SerializeField] private int columns;

    // Proxy that returns the rows and columns, so that those variables can't be changed
    public Spawner Spawner => spawner;
    public Base EndBase => endBase;
    public Path PathObject { get; private set; }
    public int Rows => rows;
    public int Columns => columns;
    public Tile SelectedTile { get; private set; }
    public GameObject indicator;
    public bool Active { get; private set; }

    // Awake is called when the game starts running
    private void Awake()
    {
        tiles = new();
        PathObject = new(this);
        Active = true;

        towerMenu.SetActive(false);

        GenerateGrid(); // Generates a new grid
        RandomiseSeed(); // Generates a path with a random seed
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

    public void SetSeed()
    {
        // Tries parsing input text from seed
        bool success = int.TryParse(seedInput.text, out int seed);

        if (success) {
            // Setting the same seed is redundant
            if (seed == PathObject.Seed)
                return;

            PathObject.Generate(seed);

            // Edge case, if input seed is negative and the seed randomises
            string seedText = PathObject.Seed.ToString();
            seedInput.text = seedText;
        } else {
            RandomiseSeed();
        }
    }

    // Generates a random seed and path
    public void RandomiseSeed()
    {
        // Generates a random path, without seed argument (will then generate a random one)
        PathObject.Generate(null);

        // Set the seed to the text box, so that players can copy seed values to replay seeds
        string seedText = PathObject.Seed.ToString();
        seedInput.text = seedText;

        // Randomising the seed while the player already selects a tile/ placed a tower could lead to problems
        // So this ensures that there are no towers placed, no selected tiles, and default cash values
        if (SelectedTile != null)
            SelectTile(SelectedTile);

        towerManager.ClearAllTowers();
        endBase.ResetBase();
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

    // Finds, and returns the tile component via address if it is found, else null is returned
    public Tile FindTileByAddress(Address address)
    {
        bool success = tiles.TryGetValue(address, out Tile tile);

        // Ternary statement shortens it to a one-liner, same logic to using if statements
        return success ? tile : null;
    }

    // Starts spawning enemies and disables being able to change the seed during active spawning
    public void StartGame()
    {
        seedInput.readOnly = true;
        buttons.SetActive(false);

        Spawner.Begin();
    }

    // Selects a tile and determines which actions are available
    public void SelectTile(Tile tile)
    {
        // Un-highlights the current selected tile
        if (SelectedTile != null)
            SelectedTile.Highlight(false);

        // If the current selected tile is being selected, deselect it
        if (SelectedTile == tile) {
            SelectedTile = null;
            towerMenu.SetActive(false);
        } else { // A new tile is being selected
            SelectedTile = tile;

            Tower occupier = SelectedTile.Occupier;
            TowerData data;

            if (occupier == null) {
                // If the tower isn't occupying the tile, allow them to purchase a selection of towers
                data = towerManager.GetDataFromType(towerManager.SelectedType);
                towerMenu.SetActive(true);
            } else {
                // Otherwise, allow them to visualise the range of the tower (from the recieved tower data)
                data = towerManager.GetDataFromType(occupier.Type);
                towerMenu.SetActive(false);
            }

            // data.range * 2 because the range is a radius, and the local scale should use diameter
            indicator.transform.localScale = new(
                data.range * 2,
                data.range * 2
            );

            // Highlights the tile
            tile.Highlight(true);
        }

        // Sets the indicator to active if there is a selected tile, else inactive
        indicator.SetActive(SelectedTile != null);

        indicator.transform.parent = tile.transform;
        indicator.transform.localPosition = new(0, 0, -4);
    }

    // End the game, true = win, false = lose
    public void End(bool isWin)
    {
        // Used to determine the interactibility of each tile
        Active = false;

        // Hide everything, show end menu (displaying restart/ menu option)
        towerMenu.SetActive(false);
        indicator.SetActive(false);
        endMenu.Show(isWin);
    }
}
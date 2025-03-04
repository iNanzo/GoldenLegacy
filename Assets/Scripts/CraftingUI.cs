using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CraftingUI : MonoBehaviour
{
    public GameObject craftingPanel;
    public TMP_Dropdown recipeDropdown;
    public Button craftButton;
    public Button closeButton;

    public GameObject materialListPanel;
    public GameObject materialButtonPrefab;
    public Button confirmCraftButton;
    public TMP_Text craftResultText;

    public Player player;
    public List<CraftingRecipe> availableRecipes = new List<CraftingRecipe>();

    private CraftingRecipe selectedRecipe;
    private List<MaterialData> selectedMaterials = new List<MaterialData>();
    private MaterialData selectedCrystal;

    void Start()
    {
        craftingPanel.SetActive(false);

        PopulateRecipeDropdown();

        recipeDropdown.onValueChanged.AddListener(OnRecipeSelected);
        craftButton.onClick.AddListener(StartMaterialSelection);
        closeButton.onClick.AddListener(() => craftingPanel.SetActive(false));
        confirmCraftButton.onClick.AddListener(ConfirmCraft);
        confirmCraftButton.interactable = false;

        craftResultText.text = "Select a recipe to begin...";
    }

    public void OpenCraftingPanel()
    {
        craftingPanel.SetActive(true);
        recipeDropdown.value = 0;
        OnRecipeSelected(0);
    }

    void PopulateRecipeDropdown()
    {
        recipeDropdown.ClearOptions();
        List<string> recipeNames = new List<string>();

        foreach (var recipe in availableRecipes)
        {
            recipeNames.Add(recipe.itemType.ToString());
        }

        recipeDropdown.AddOptions(recipeNames);

        if (availableRecipes.Count > 0)
        {
            selectedRecipe = availableRecipes[0];
        }
    }

    void OnRecipeSelected(int index)
    {
        selectedRecipe = availableRecipes[index];
        Debug.Log($"Selected recipe: {selectedRecipe.itemType}");
        craftResultText.text = $"Preparing to craft a {selectedRecipe.itemType}...";
    }

    void StartMaterialSelection()
    {
        selectedMaterials.Clear();
        selectedCrystal = null;
        RefreshMaterialList();
        UpdateCraftPreview();
    }

    void RefreshMaterialList()
    {
        foreach (Transform child in materialListPanel.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var material in player.inventory.GetInventoryItems())
        {
            GameObject buttonObj = Instantiate(materialButtonPrefab, materialListPanel.transform);
            TMP_Text buttonText = buttonObj.GetComponentInChildren<TMP_Text>();
            buttonText.text = material.GetMaterialDescription();

            Button button = buttonObj.GetComponent<Button>();
            button.onClick.AddListener(() => OnMaterialSelected(material));
        }

        UpdateCraftPreview();
    }

    void OnMaterialSelected(MaterialData material)
    {
        int requiredCount = GetRequiredMaterialCount();

        if (selectedMaterials.Contains(material))
        {
            selectedMaterials.Remove(material);
            Debug.Log($"Deselected Material: {material.GetMaterialDescription()}");
        }
        else if (selectedCrystal == material)
        {
            selectedCrystal = null;
            Debug.Log($"Deselected Crystal: {material.GetMaterialDescription()}");
        }
        else if (selectedMaterials.Count < requiredCount)
        {
            selectedMaterials.Add(material);
            Debug.Log($"Selected Material: {material.GetMaterialDescription()}");
        }
        else if (CanUseCrystalSlot() && selectedCrystal == null)
        {
            selectedCrystal = material;
            Debug.Log($"Selected Crystal: {material.GetMaterialDescription()}");
        }
        else
        {
            Debug.Log("Material slots are full!");
            return;
        }

        UpdateCraftPreview();
    }

    void UpdateCraftPreview()
    {
        int requiredCount = GetRequiredMaterialCount();
        List<string> selectedDescriptions = new List<string>();

        foreach (var material in selectedMaterials)
        {
            selectedDescriptions.Add(material.GetMaterialDescription());
        }

        string crystalText = selectedCrystal != null
            ? $"\nCrystal:\n- {selectedCrystal.GetMaterialDescription()}"
            : "\nCrystal:\nNone";

        string result = "Selected Materials:\n";
        result += selectedDescriptions.Count > 0
            ? "- " + string.Join("\n- ", selectedDescriptions)
            : "None";

        result += crystalText;

        int missingMaterials = requiredCount - selectedMaterials.Count;
        List<string> missingParts = new List<string>();
        if (missingMaterials > 0)
            missingParts.Add($"{missingMaterials} more material{(missingMaterials > 1 ? "s" : "")}");

        if (CanUseCrystalSlot() && selectedCrystal == null)
            missingParts.Add("Crystal (optional)");

        if (missingParts.Count > 0)
        {
            result += "\n\nMissing:\n- " + string.Join("\n- ", missingParts);
        }

        if (selectedMaterials.Count == requiredCount)
        {
            int totalAttack = selectedRecipe.GetTotalAttack();
            int totalHP = selectedRecipe.GetTotalHP();
            int totalValue = selectedRecipe.GetTotalGoldValue();

            result += $"\n\nReady to craft: {selectedRecipe.itemType}\n" +
                      $"Attack: {totalAttack}, HP: {totalHP}, Value: {totalValue}g";
            confirmCraftButton.interactable = true;
        }
        else
        {
            result += "\n\nWaiting for more materials...";
            confirmCraftButton.interactable = false;
        }

        craftResultText.text = result;
    }

    void ConfirmCraft()
    {
        if (selectedMaterials.Count != GetRequiredMaterialCount())
        {
            Debug.LogError("Not enough materials selected!");
            return;
        }

        foreach (var material in selectedMaterials)
        {
            Debug.Log($"Removing Material from inventory: {material.GetMaterialDescription()}");
            player.inventory.RemoveItem(material);
        }
        if (selectedCrystal != null)
        {
            Debug.Log($"Removing Crystal from inventory: {selectedCrystal.GetMaterialDescription()}");
            player.inventory.RemoveItem(selectedCrystal);
        }

        player.GetComponent<CraftingSystem>().CraftItem(selectedRecipe, selectedMaterials.ToArray(), selectedCrystal);
        craftingPanel.SetActive(false);

        craftResultText.text = $"{selectedRecipe.itemType} added to inventory!";
    }

    int GetRequiredMaterialCount()
    {
        switch (selectedRecipe.itemType)
        {
            case CraftableItemType.Sword:
            case CraftableItemType.Armor:
                return 2;
            case CraftableItemType.Helmet:
            case CraftableItemType.Ring:
                return 1;
            default:
                return 0;
        }
    }

    bool CanUseCrystalSlot()
    {
        return selectedRecipe.itemType == CraftableItemType.Sword
            || selectedRecipe.itemType == CraftableItemType.Helmet
            || selectedRecipe.itemType == CraftableItemType.Ring;
    }
}

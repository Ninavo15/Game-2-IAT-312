using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// One-off wiring for the batch of interaction/ambience sounds requested
// across Scene 3, Scene 4, Scene 6, Home, Selection Object, Wipe Mini Game
// and Ripping poster. Run each BuildX method via -executeMethod, then
// delete this file.
public static class AudioSceneWiring
{
    // --- Clip guids ---
    const string CarClose = "7d9107b4f520b4077a614be19f8ed705";
    const string FuelPump = "ce6c2d0f4bce64a44b7cb36ef9105176";
    const string DoorOpen = "9d7c252edc8ba45efbfa1b9050697951";
    const string Wipping = "8bb1e9504d13a40fd9d32ab67e215e48";
    const string Tear = "a46cbf63b0e6a46ebb5cc78cc5a02113";
    const string ObjectChose = "96cbd987b4f0640a3be37b950975423d";
    const string DoorOpenClose = "615e36936044e4b4a84501619c6ead2b";
    const string TvNoise = "b8d6263a4e3f54d488ba7482690662a4";
    const string Breathing = "a8cc23716c26e144db52796edbd14ba1";
    const string HeartBeat = "cdb6d6538a8680342afe79343a9f165f";
    const string JumpScare = "cd1b7145687b7d14cb6e61cfeb3e5f7d";
    const string Scream = "b34161c36dce941258c07815b03a59ff";
    const string GhostAppear = "0e86a8cb05a7b4123a55b5cb58247d99";

    public static void BuildScene3()
    {
        var scene = Open("Assets/Scenes/Scene 3.unity");

        var carMovement = FindComponent<CarMovement>(scene);
        WireAudio(carMovement.gameObject, "doorSound", "doorCloseClip", carMovement, CarClose);

        var carEntry = FindComponent<CarEntryInteraction>(scene);
        WireAudio(carEntry.gameObject, "doorSound", "doorCloseClip", carEntry, CarClose);

        foreach (var interactable in FindInteractablesByPrompt(scene, "Gas Pump Interaction Box"))
        {
            WireAudio(interactable.gameObject, "interactSound", "interactClip", interactable, FuelPump);
        }
        foreach (var interactable in FindInteractablesByPrompt(scene, "Store Door Interaction Box"))
        {
            WireAudio(interactable.gameObject, "interactSound", "interactClip", interactable, DoorOpen);
        }

        SaveAndLog(scene);
    }

    public static void BuildScene4()
    {
        var scene = Open("Assets/Scenes/Scene 4.unity");

        foreach (var interactable in FindInteractablesByPrompt(scene, "Store Door Interaction Box"))
        {
            WireAudio(interactable.gameObject, "interactSound", "interactClip", interactable, DoorOpen);
        }

        SaveAndLog(scene);
    }

    public static void BuildScene6()
    {
        var scene = Open("Assets/Scenes/Scene 6 (outside home.unity");

        var carMovement = FindComponent<CarMovement>(scene);
        WireAudio(carMovement.gameObject, "doorSound", "doorCloseClip", carMovement, CarClose);

        foreach (var interactable in FindInteractablesByPrompt(scene, "House Door Interaction Box"))
        {
            WireAudio(interactable.gameObject, "interactSound", "interactClip", interactable, DoorOpenClose);
        }

        SaveAndLog(scene);
    }

    public static void BuildHome()
    {
        var scene = Open("Assets/Scenes/Home.unity");

        var intro = FindComponent<HomeIntroSequence>(scene);
        GameObject go = intro.gameObject;
        WireAudio(go, "tvAudio", "tvNoiseClip", intro, TvNoise, loop: true);
        WireAudio(go, "ghostAppearAudio", "ghostAppearClip", intro, GhostAppear);
        WireAudio(go, "breathingAudio", "breathingClip", intro, Breathing, loop: true);
        WireAudio(go, "heartbeatAudio", "heartbeatClip", intro, HeartBeat, loop: true);
        WireAudio(go, "jumpscareAudio", "jumpscareClip", intro, JumpScare);
        WireAudio(go, "screamAudio", "screamClip", intro, Scream);

        SaveAndLog(scene);
    }

    public static void BuildSelectionObject()
    {
        var scene = Open("Assets/Scenes/Mini Game/Selection Object.unity");

        var selection = FindComponent<SelectionMiniGame>(scene);
        WireAudio(selection.gameObject, "chooseAudio", "chooseClip", selection, ObjectChose);

        SaveAndLog(scene);
    }

    public static void BuildWipeMiniGame()
    {
        var scene = Open("Assets/Scenes/Mini Game/Wipe Mini Game.unity");

        var wiping = FindComponent<Wipping>(scene);
        WireAudio(wiping.gameObject, "wipeAudio", "wipeClip", wiping, Wipping);

        SaveAndLog(scene);
    }

    public static void BuildRippingPoster()
    {
        var scene = Open("Assets/Scenes/Mini Game/Ripping poster.unity");

        var rip = FindComponent<PosterRipMiniGame>(scene);
        WireAudio(rip.gameObject, "tearAudio", "tearClip", rip, Tear);

        SaveAndLog(scene);
    }

    // --- Helpers ---

    static Scene Open(string path) => EditorSceneManager.OpenScene(path);

    static void SaveAndLog(Scene scene)
    {
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log($"AudioSceneWiring: {scene.path} wired and saved successfully.");
    }

    static T FindComponent<T>(Scene scene) where T : Component
    {
        foreach (var root in scene.GetRootGameObjects())
        {
            var found = root.GetComponentInChildren<T>(true);
            if (found != null) return found;
        }
        throw new System.Exception($"{typeof(T).Name} not found in {scene.path}");
    }

    // Interactable lives on a trigger object separate from its prompt UI
    // panel - find it by which prompt it points at instead of by its own
    // name. Some prompts have more than one Interactable pointing at them
    // (duplicate trigger objects), so this returns all matches.
    static List<Interactable> FindInteractablesByPrompt(Scene scene, string promptName)
    {
        var matches = new List<Interactable>();
        foreach (var root in scene.GetRootGameObjects())
        {
            foreach (var interactable in root.GetComponentsInChildren<Interactable>(true))
            {
                if (interactable.promptText != null && interactable.promptText.name == promptName)
                {
                    matches.Add(interactable);
                }
            }
        }
        if (matches.Count == 0) throw new System.Exception($"No Interactable found with promptText '{promptName}' in {scene.path}");
        return matches;
    }

    // Adds a new AudioSource to go, wires it to clipGuid, and assigns it plus
    // the clip onto the named fields of target via SerializedObject.
    static void WireAudio(GameObject go, string audioSourceField, string clipField, Object target, string clipGuid, bool loop = false)
    {
        AudioSource src = go.AddComponent<AudioSource>();
        src.playOnAwake = false;
        src.loop = loop;
        AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(AssetDatabase.GUIDToAssetPath(clipGuid));
        src.clip = clip;

        var so = new SerializedObject(target);
        so.FindProperty(audioSourceField).objectReferenceValue = src;
        so.FindProperty(clipField).objectReferenceValue = clip;
        so.ApplyModifiedPropertiesWithoutUndo();
    }
}

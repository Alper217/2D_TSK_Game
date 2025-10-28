// TribuneManager.cs (G�ncellenmi� Hali)
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TribuneManager : MonoBehaviour
{
    [Tooltip("Sahnedeki T�M SupporterGroup script'lerini buraya s�r�kleyin")]
    public List<SupporterGroup> allSupporterGroups = new List<SupporterGroup>();

    [Header("Gol Sevinci Ayarlar�")]
    public float celebrationDuration = 2.0f;
    public float celebrationJumpHeight = 0.15f;
    public float celebrationSpeed = 25.0f;

    [Header("Meksika Dalgas� Ayarlar�")]
    public float waveJumpHeight = 0.3f;
    public float waveJumpDuration = 0.6f;
    public float waveDelayBetweenFans = 0.05f;
    public float waveDelayBetweenGroups = 0.2f;

    void Start()
    {
        // Otomatik bulma
        if (allSupporterGroups.Count == 0)
        {
            allSupporterGroups = FindObjectsOfType<SupporterGroup>().ToList();
            Debug.Log($"Otomatik olarak {allSupporterGroups.Count} taraftar grubu bulundu.");
        }

        // S�ralamay� kald�rd���m�z kod (�nceki ad�mdaki gibi kalmal�)
        //allSupporterGroups = allSupporterGroups.OrderBy(group => group.transform.position.x).ToList();
    }

    /// <summary>
    /// GameManager taraf�ndan �a�r�l�r.
    /// Gol atan tak�ma ait trib�nleri sevindirir.
    /// </summary>
    public void TriggerCelebration(Team teamThatScored)
    {
        foreach (SupporterGroup group in allSupporterGroups)
        {
            if (group.supportedTeam == teamThatScored)
            {
                group.TriggerCelebration(celebrationDuration, celebrationJumpHeight, celebrationSpeed);
            }
        }
    }

    /// <summary>
    /// Meksika dalgas�n� ba�lat�r.
    /// (Art�k sadece GameManager taraf�ndan �a�r�l�yor)
    /// </summary>
    public void StartMexicanWave()
    {
        float currentGroupDelay = 0f;

        foreach (SupporterGroup group in allSupporterGroups)
        {
            group.TriggerWave(currentGroupDelay, waveJumpHeight, waveJumpDuration);
            currentGroupDelay += waveDelayBetweenGroups;
        }
    }

    // --- TEST ���N OLAN UPDATE FONKS�YONU KALDIRILDI ---
    // (Art�k GameManager'da)
}
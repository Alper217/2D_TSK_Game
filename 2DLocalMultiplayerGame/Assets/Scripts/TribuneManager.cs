// TribuneManager.cs (Güncellenmiþ Hali)
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TribuneManager : MonoBehaviour
{
    [Tooltip("Sahnedeki TÜM SupporterGroup script'lerini buraya sürükleyin")]
    public List<SupporterGroup> allSupporterGroups = new List<SupporterGroup>();

    [Header("Gol Sevinci Ayarlarý")]
    public float celebrationDuration = 2.0f;
    public float celebrationJumpHeight = 0.15f;
    public float celebrationSpeed = 25.0f;

    [Header("Meksika Dalgasý Ayarlarý")]
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

        // Sýralamayý kaldýrdýðýmýz kod (Önceki adýmdaki gibi kalmalý)
        //allSupporterGroups = allSupporterGroups.OrderBy(group => group.transform.position.x).ToList();
    }

    /// <summary>
    /// GameManager tarafýndan çaðrýlýr.
    /// Gol atan takýma ait tribünleri sevindirir.
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
    /// Meksika dalgasýný baþlatýr.
    /// (Artýk sadece GameManager tarafýndan çaðrýlýyor)
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

    // --- TEST ÝÇÝN OLAN UPDATE FONKSÝYONU KALDIRILDI ---
    // (Artýk GameManager'da)
}
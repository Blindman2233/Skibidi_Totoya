using UnityEngine;
using DS.ScriptableObjects;

namespace DS
{
    public class DSDialogueTrigger : MonoBehaviour
    {
        [Header("Dialogue Settings")]
        public DSDialogueContainerSO dialogueContainer;
        public string dialogueName = ""; // ถ้าเว้นว่างจะเริ่มจาก starting dialogue แรก
        public bool triggerOnEnter = true;
        public bool requireKeyPress = true;
        public KeyCode interactionKey = KeyCode.E;

        [Header("Visual Indicator")]
        public GameObject interactionIndicator;
        public float indicatorOffset = 2f;

        private bool canInteract = false;
        private GameObject player;

        private void Start()
        {
            if (interactionIndicator != null)
            {
                interactionIndicator.SetActive(false);
            }
        }

        private void Update()
        {
            if (canInteract && requireKeyPress && Input.GetKeyDown(interactionKey))
            {
                StartDialogue();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                player = other.gameObject;
                canInteract = true;

                if (interactionIndicator != null)
                {
                    interactionIndicator.SetActive(true);
                    interactionIndicator.transform.position = transform.position + Vector3.up * indicatorOffset;
                }

                if (triggerOnEnter && !requireKeyPress)
                {
                    StartDialogue();
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                canInteract = false;
                player = null;

                if (interactionIndicator != null)
                {
                    interactionIndicator.SetActive(false);
                }
            }
        }

        public void StartDialogue()
        {
            if (DSDialogueManager.Instance != null)
            {
                // ถ้า dialogueName ว่าง จะเริ่มจาก starting dialogue แรกของ container
                DSDialogueManager.Instance.StartDialogueFromContainer(dialogueContainer, dialogueName);
            }
            else
            {
                Debug.LogError("DSDialogueManager instance not found!");
            }
        }

        private void OnDrawGizmos()
        {
            // วาด gizmo แสดงพื้นที่ trigger
            Gizmos.color = Color.yellow;
            Collider2D collider = GetComponent<Collider2D>();
            if (collider != null)
            {
                Gizmos.DrawWireCube(transform.position, collider.bounds.size);
            }
        }
    }
}

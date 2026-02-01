using System;
using UnityEngine;

public class GateController : MonoBehaviour
{
        public Sprite closedSprite;
        public Sprite openSprite;
        private bool isOpen = false;
        private SpriteRenderer spriteRenderer;

        public static GateController Instance;

        private void Awake() {
            if (Instance == null) {
                Instance = this;
            }
        }

        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            SetGateState(true);
        }
    
        public void SetGateState(bool open)
        {
            isOpen = open;
            spriteRenderer.sprite = isOpen ? openSprite : closedSprite;
            if (isOpen)
            {
                AdjustColliderForOpenGate();
            }
        }
        
        private void AdjustColliderForOpenGate()
        {
            BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
            if (boxCollider != null)
            {
                boxCollider.size = new Vector2(1.7f, boxCollider.size.y);
                boxCollider.offset = new Vector2(1.7f, boxCollider.offset.y);
            }
        }
}

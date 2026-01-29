using UnityEngine;

public class GateController : MonoBehaviour
{
        public Sprite closedSprite;
        public Sprite openSprite;
        private bool isOpen = false;
        private SpriteRenderer spriteRenderer;
        
        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            SetGateState(isOpen);
        }
    
        public void SetGateState(bool open)
        {
            isOpen = open;
            spriteRenderer.sprite = isOpen ? openSprite : closedSprite;
            BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
            boxCollider.offset.Set(1.7f, boxCollider.offset.y);
            boxCollider.size.Set(1.7f, boxCollider.size.y);
        }

        public bool IsOpen => isOpen;
}

shader_type canvas_item;

uniform float progress : hint_range(-3, 3);
uniform float diamondPixelSize = 10.0;
uniform vec2 transitionDirection = vec2(1.0, 1.0);
uniform float rotationAngle : hint_range(0, 360); // Degrees

void fragment() {
    float xFraction = fract(FRAGCOORD.x / diamondPixelSize);
    float yFraction = fract(FRAGCOORD.y / diamondPixelSize);
    float xDistance = abs(xFraction - 0.5);
    float yDistance = abs(yFraction - 0.5);
    
    // Convert degrees to radians for the rotation angle
    float radians = rotationAngle * (PI / 180.0); // Conversion formula
    
    // Apply rotation to the transition direction vector
    float cosAngle = cos(radians); // Use the converted radians
    float sinAngle = sin(radians); // Use the converted radians
    vec2 rotatedDirection = vec2(
        transitionDirection.x * cosAngle - transitionDirection.y * sinAngle,
        transitionDirection.x * sinAngle + transitionDirection.y * cosAngle
    );
    
    // Normalize the rotated direction vector
    vec2 normDirection = normalize(rotatedDirection);
    
    // Adjust the calculation to better map the effect across the full node
    float adjustedProgress = progress * 4.0 * max(abs(normDirection.x), abs(normDirection.y));
    
    if (xDistance + yDistance > adjustedProgress - (normDirection.x * UV.x + normDirection.y * UV.y) * 4.0) {
        discard;
    }
}

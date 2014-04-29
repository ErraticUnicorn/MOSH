
public class Light {
    private Vector2 pos;
    private float brightness;
    private bool on;
    private const float K = 100.0f; //distance mod constant
    static const float K_2 = 1.0f; //overall effect constant. Higher means lights are stronger

    public Light(Vector2 p, float b){
        pos = p;
        brightness = b;
        on = true;
}
public void Switch(){
    on = !on;
}
public float GetBrightness(){
    if(on)
        return brightness;
    else
        return 0.0f;
}
public float GetDistanceMod(Vector2 otherPos){
    //all light sources are equally strong once you’re within K of them
    float d = Vector2.Distance(pos, otherPos);
    d = K / d;
    if (d > 1.0f)
        d = 1.0f;
    return d;
}
//use this color instead of Color.White in the arguments to SpriteBatch.Draw
static Color GetTintedColor(Light[] lights, Vector2 myPos){
    float lumes = 0.0f;
    foreach (Light light in lights) {
        lumens += light.GetBrightness() * light.GetDistanceMod(myPos);
    }
    lumens *= K_2; //some constant
    if (lumens > 1.0f)
        lumens = 1.0f;
    Color color = Color.White;
    float value = 255*lumens;
        color.r = value;
        color.g = value;
        color.b = value;
        color.a = 1.0f;
    return color;
}
}



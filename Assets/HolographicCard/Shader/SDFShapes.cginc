#ifndef SDF_SHAPES
#define SDF_SHAPES

half pow2(half val)
{
    return val * val;
}

half pow2(half3 val)
{
    return val * val;
}

half FluidStep(half v, half threshold)
{
    //return saturate(v / fwidth(v) - threshold);
    return saturate(smoothstep(0.02, 0.03, v) - threshold);
}

half FluidStep(half v)
{
    //return saturate(v / fwidth(v));
    return saturate(smoothstep(0.02, 0.04, v));
}

half2 FluidStep(half2 v)
{
    //return saturate(v / fwidth(v));
    return saturate(smoothstep(0.02, 0.03, v));
}

half3 FluidStep(half3 v)
{
    //return saturate(v / fwidth(v));
    return saturate(smoothstep(0.02, 0.03, v));
}

half SmoothMin(half a, half b, half k)
{
    const half h = saturate(0.5 + 0.5 * (a - b) / k);
    return lerp(a, b, h) - k * h * (1 - h);
}

void DrawSDFIncrement(half val, inout half Base)
{
    Base += FluidStep(val);
}

void DrawSDF(half val, inout half4 BaseColor, half4 NextColor)
{
    BaseColor.rgb = lerp(BaseColor.rgb, NextColor.rgb, FluidStep(val) * NextColor.a);
}

void DrawSDF4(half val, inout half4 BaseColor, half4 NextColor)
{
    BaseColor = lerp(BaseColor, NextColor, FluidStep(val));
}

void DrawSDFDarkShade(half val, inout half4 BaseColor, half bVal)
{
    BaseColor.rgb *= lerp(1, bVal, FluidStep(val));
}

void DrawSDFLightShade(half val, inout half4 BaseColor, half lVal)
{
    BaseColor.rgb *= lerp(1, 2 - lVal, FluidStep(val));
}

void DrawSDFErase(half val, inout half4 BaseColor)
{
    BaseColor.a *= lerp(1, 0, FluidStep(val));
}

void DrawSDFInverse(half val, inout half4 BaseColor, half4 NextColor)
{
    BaseColor.rbg = lerp(BaseColor.rgb, NextColor.rgb, (1 - FluidStep(val)) * NextColor.a);
}

half Segment(half2 uv, half2 a, half2 b)
{
    half2 pa = uv - a;
    half2 ba = b - a;
    half h = saturate(dot(pa, ba) / dot(ba, ba));
    return length(pa - ba * h);
}

half2 Rotate(half2 m, half r)
{
    half c = cos(r);
    half s = sin(r);
    return mul(half2x2(c, -s, s, c), m);
}

half CurvedRectangle(half2 uv, half2 size, half radius)
{
    half2 q = abs(uv) - size + radius;
    return length(max(q, 0)) + min(max(q.x, q.y), 0) - radius;
}

half CurvedRectangleVarCorner(half2 uv, half2 size, half4 radii)
{
    radii.xy = (uv.x > 0) ? radii.xw : radii.yz;
    radii.x = (uv.y > 0) ? radii.x : radii.y;
    return CurvedRectangle(uv, size, radii.x);
}

half Square(half2 uv, half a)
{
    uv = abs(uv) - a;
    return length(max(uv, 0)) + min(max(uv.x, uv.y), 0);
}

half Ellipse(half2 uv, half2 offset, half2 scale, half rotation)
{
    const half c = cos(rotation);
    const half s = sin(rotation);
    return 1 -
        pow2((uv.x - offset.x) * c - (uv.y - offset.y) * s) / scale.x -
        pow2((uv.x - offset.x) * s + (uv.y - offset.y) * c) / scale.y;
}

#define SQRT3 1.7320508

half Egg(half2 uv, half ra, half rb)
{
    uv.x = abs(uv.x);
    half r = ra - rb;
    return ((uv.y < 0.0) ? length(half2(uv.x, uv.y)) - r : (SQRT3 * (uv.x + r) < uv.y) ? length(half2(uv.x, uv.y - SQRT3 * r)) : length(half2(uv.x + r, uv.y)) - 2.0 * r) - rb;
}

half Parallelogram(half2 uv, half width, half height, half skew)
{
    half2 e = half2(skew, height);
    uv *= sign(uv.y);
    half2 w = uv - e;
    w.x -= clamp(w.x, -width, width);
    half2 d = half2(dot(w, w), -w.y);
    half s = uv.x * e.y - uv.y * e.x;
    uv *= sign(s);
    half2 v = uv - half2(width, 0);
    v -= e * clamp(dot(v, e) / dot(e, e), -1, 1);
    d = min(d, half2(dot(v, v), width * height - abs(s)));
    return sqrt(d.x) * sign(-d.y);
}

half RParallelogram(half2 uv, half width, half height, half skew)
{
    uv.x *= saturate(sign(uv.x));
    return Parallelogram(uv, width, height, skew);
}

half LParallelogram(half2 uv, half width, half height, half skew)
{
    uv.x *= saturate(sign(-uv.x));
    return Parallelogram(uv, width, height, skew);
}

half RegularHexagon(half2 uv, half r)
{
    const half3 k = half3(-0.866025404, 0.5, 0.577350269);
    uv = abs(uv);
    uv -= 2 * min(dot(k.xy, uv), 0) * k.xy;
    uv -= half2(clamp(uv.x, -k.z * r, k.z * r), r);
    return length(uv) * sign(uv.y);
}

half Hexagon(half2 uv, half r, half3 k)
{
    uv = abs(uv);
    uv -= 2 * min(dot(k.xy, uv), 0) * k.xy;
    uv -= half2(clamp(uv.x, -k.z * r, k.z * r), r);
    return length(uv) * sign(uv.y);
}

half ndot(half2 a, half2 b)
{
    return a.x * b.x - a.y * b.y;
}

half Rhombus(half2 uv, half2 b)
{
    uv = abs(uv);
    half h = clamp(ndot(b - 2 * uv, b) / dot(b, b), -1, 1);
    half d = length(uv - 0.5 * b * half2(1 - h, 1 + h));
    return d * sign(uv.x * b.y + uv.y * b.x - b.x * b.y);
}

half Trapezoid(half2 uv, half r1, half r2, half height)
{
    half2 k1 = half2(r2, height);
    half2 k2 = half2(r2 - r1, 2 * height);
    uv.x = abs(uv.x);
    half2 ca = half2(uv.x - min(uv.x, (uv.y < 0) ? r1 : r2), abs(uv.y) - height);
    half2 cb = uv - k1 + k2 * saturate(dot(k1 - uv, k2) / dot(k2, k2));
    return (max(sign(cb.x), sign(ca.y))) * sqrt(min(dot(ca, ca), dot(cb, cb)));
}

half ExtendedHexagon(half2 uv, half r1, half r2, half height)
{
    height /= 2.0;
    half2 k1 = half2(r2, height);
    half2 k2 = half2(r2 - r1, 2.0 * height);
    uv = abs(uv);
    uv.y -= height;
    half2 ca = half2(uv.x - min(uv.x, r2), uv.y - height);
    half2 cb = uv - k1 + k2 * saturate(dot(k1 - uv, k2) / dot(k2, k2));
    half s = (cb.x < 0.0 && ca.y < 0.0) ? - 1.0 : 1.0;
    return s * sqrt(min(dot(ca, ca), dot(cb, cb)));
}


half Circle(half2 uv, half r)
{
    return length(uv) - r;
}

half Circle(half2 uv, half r, half2 c)
{
    return length(uv - c) - r;
}

half CircleInverse(half2 uv, half r, half d)
{
    uv = abs(uv);
    if (uv.y > uv.x) uv = uv.yx;
    return Circle(uv - half2(d, 0), r);
}

//cv = checkervector, cf = checkerfill rot = rotation
half CheckerBoard(half2 uv, inout half2 uvAdj, half2 cv, half2x2 rot, half scale)
{
    uvAdj = uv - _Time.x * cv;
    //uvAdj = mul(uvAdj, rot);
    half mVal = abs(sign(uvAdj.x) - sign(uvAdj.y)) * 0.5;
    uvAdj = abs(uvAdj) * scale;
    half sVal = abs(floor(uvAdj.x) - floor(uvAdj.y)) % 2;
    sVal = (sVal + mVal) % 2;
    uvAdj = frac(uvAdj) - 0.5;
    return sVal;
}

half StaticCheckerBoard(inout half2 uv, half scale)
{
    const half mVal = abs(sign(uv.x) - sign(uv.y)) * 0.5;
    uv = abs(uv) * scale;
    half sVal = abs(floor(uv.x) - floor(uv.y)) % 2;
    sVal = (sVal + mVal) % 2;
    uv = frac(uv) - 0.5;
    return sVal;
}

half CheckerBoardSquare(half2 uv, half4 cv, half cf, half2x2 rot, half scale)
{
    half2 uvAdj;
    half sVal = CheckerBoard(uv, uvAdj, cv.zw, rot, scale);
    //i.uv *= cv.xy;
    //sVal = sVal * -Square(uvAdj, _CheckerFill - i.uv.x - i.uv.y) + (sVal-1) * -Square(uvAdj, 1 - (_CheckerFill - i.uv.x - i.uv.y));
    half tVal = 2 * sVal - 1;
    sVal = tVal * - Square(uvAdj, (1 - sVal) + tVal * (cf - uv.x * cv.x - uv.y * cv.y));
    return sVal;
}

half CheckerBoardSquareUniform(half2 uv, half2 cv, half cf, half2x2 rot, half scale)
{
    half2 uvAdj;
    half sVal = CheckerBoard(uv, uvAdj, cv, rot, scale);
    half tVal = 2 * sVal - 1;
    sVal = tVal * - Square(uvAdj, (1 - sVal) + tVal * cf);
    return sVal;
}

half CheckerBoardCircleUniform(half2 uv, half2 cv, half cf, half2x2 rot, half scale)
{
    half2 uvAdj;
    half sVal = CheckerBoard(uv, uvAdj, cv, rot, scale);
    if (sVal > 0.5)
        sVal = -Circle(uvAdj, cf);
    else
        sVal = -CircleInverse(uvAdj, cf, 1);
    return sVal;
}

half CheckerBoardCircleUniformFast(half2 uv, half2 cv, half cf, half scale)
{
    uv *= scale;
    uv -= _Time.x * cv;
    uv = abs(uv) * scale;
    const half checker_val = abs(floor(uv.x) - floor(uv.y)) % 2;
    uv = frac(uv) - 0.5;
    return -Circle(uv, cf) * checker_val;
}

half StippleEffect(half2 uv, half2 facingDirection, half2 movingDirection, half fill, half scale)
{
    half2 checkerUV = uv - _Time.x * movingDirection;
    const half mVal = abs(sign(checkerUV.x) - sign(checkerUV.y)) * 0.5;
    checkerUV = abs(checkerUV) * scale;
    half sVal = abs(floor(checkerUV.x) - floor(checkerUV.y)) % 2;
    sVal = (sVal + mVal) % 2;
    checkerUV = frac(checkerUV) - 0.5;
    uv *= facingDirection;
    if (sVal > 0.5) return -Circle(checkerUV, fill - uv.x - uv.y);
    return -CircleInverse(checkerUV, fill - uv.x - uv.y, 1);
}

half EquilateralTriangle(half2 uv)
{
    const half k = 1.732050808;
    uv.x = abs(uv.x) - 1;
    uv.y = uv.y + 1 / k;
    if (uv.x + k * uv.y > 0) uv = half2(uv.x - k * uv.y, -k * uv.x - uv.y) / 2;
    uv.x -= clamp(uv.x, -2, 0);
    return -length(uv) * sign(uv.y);
}

half CurvedOutline(half csValue, half lineWidth)
{
    return csValue - lineWidth;
}

half OcticChunk(half2 uv, half a, half f)
{
    return uv.y - (a * pow(uv.x, 8) + f);
}

half HorizontalLinearChunkEstimate(half2 uv, half a, half b)
{
    return uv.y - (a * uv.x + b);
}

half HorizontalLineEstimate(half2 uv, half a, half b, half width)
{
    return -abs(HorizontalLinearChunkEstimate(uv, a, b)) + width;
}

half VerticalLinearChunkEstimate(half2 uv, half a, half b)
{
    return uv.x - (a * uv.y + b);
}

half VerticalLineEstimate(half2 uv, half a, half b, half width)
{
    return -abs(VerticalLinearChunkEstimate(uv, a, b)) + width;
}

half HorizontalLine(half2 uv, half a, half b)
{
    const half x = (uv.x + a * uv.y - a * b) / (1 + a * a);
    const half y = a * x + b;
    return length(uv - half2(x, y));
}

half HorizontalLinearChunk(half2 uv, half a, half b)
{
    return HorizontalLine(uv, a, b) * sign(uv.y - (a * uv.x + b));
}

half VerticalLine(half2 uv, half a, half b)
{
    const half y = (uv.y + a * uv.x - a * b) / (1 + a * a);
    const half x = a * y + b;
    return length(uv - half2(x, y));
}

half VerticalLinearChunk(half2 uv, half a, half b)
{
    return VerticalLine(uv, a, b) * sign(uv.x - (a * uv.y + b));
}

float dot2(in half2 v) { return dot(v, v); }
float cro(in half2 a, in half2 b) { return a.x * b.y - a.y * b.x; }

half Bezier(const half2 uv, const half2 pointA, const half2 pointB, const half2 pointC)
{
    const half2 a = pointB - pointA;
    const half2 b = pointA - 2.0 * pointB + pointC;
    const half2 c = a * 2.0;
    const half2 d = pointA - uv;

    const float kk = 1.0 / dot(b, b);
    const float kx = kk * dot(a, b);
    const float ky = kk * (2.0 * dot(a, a) + dot(d, b)) / 3.0;
    const float kz = kk * dot(d, a);

    float res;
    float sgn;

    const float p = ky - kx * kx;
    const float q = kx * (2.0 * kx * kx - 3.0 * ky) + kz;
    const float p3 = p * p * p;
    const float q2 = q * q;
    float h = q2 + 4.0 * p3;

    if (h >= 0.0)
    {
        // 1 root
        h = sqrt(h);
        half2 x = (half2(h, -h) - q) / 2.0;

        #if 0
        // When p≈0 and p<0, h-q has catastrophic cancelation. So, we do
        // h=√(q²+4p³)=q·√(1+4p³/q²)=q·√(1+w) instead. Now we approximate
        // √ by a linear Taylor expansion into h≈q(1+½w) so that the q's
        // cancel each other in h-q. Expanding and simplifying further we
        // get x=vec2(p³/q,-p³/q-q). And using a second degree Taylor
        // expansion instead: x=vec2(k,-k-q) with k=(1-p³/q²)·p³/q
        if( abs(p)<0.001 )
        {
            float k = p3/q;              // linear approx
          //float k = (1.0-p3/q2)*p3/q;  // quadratic approx 
            x = vec2(k,-k-q);  
        }
        #endif

        half2 xy = sign(x) * pow(abs(x), half2(1, 1) / 3);
        const float t = clamp(xy.x + xy.y - kx, 0, 1);
        const half2 q_s = d + (c + b * t) * t;
        res = dot2(q_s);
        sgn = cro(c + 2.0 * b * t, q_s);
    }
    else
    {
        // 3 roots
        const float z = sqrt(-p);
        const float v = acos(q / (p * z * 2.0)) / 3.0;
        const float m = cos(v);
        const float n = sin(v) * 1.732050808;
        half2 t = clamp(half3(m + m, -n - m, n - m) * z - kx, 0.0, 1.0);
        const half2 qx = d + (c + b * t.x) * t.x;
        const float dx = dot2(qx);
        const float sx = cro(c + 2.0 * b * t.x, qx);
        const half2 qy = d + (c + b * t.y) * t.y;
        const float dy = dot2(qy);
        const float sy = cro(c + 2.0 * b * t.y, qy);
        if (dx < dy)
        {
            res = dx;
            sgn = sx;
        }
        else
        {
            res = dy;
            sgn = sy;
        }
    }
    return sqrt(res) * sign(sgn);
}
#endif

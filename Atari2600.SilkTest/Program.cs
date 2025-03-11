// See https://aka.ms/new-console-template for more information

using Silk.NET.Core.Native;
using Silk.NET.Direct2D;
using Silk.NET.DXGI;
using Silk.NET.Maths;
using Silk.NET.Windowing;

Console.WriteLine("Hello, World!");

WindowOptions Options = WindowOptions.Default with {
                                                       Size = new Vector2D<int>(600, 800),
                                                       Title = "My Test Window",
                                                       TransparentFramebuffer = true
                                                   };

IWindow MyWindow = Window.Create(Options);

ComPtr<ID2D1Factory> Factory = new();
ComPtr<ID2D1HwndRenderTarget> Target = new();
ComPtr<ID2D1Brush> Brush = null;

MyWindow.Render += MyWindow_Render;
MyWindow.Load += MyWindow_Load;

unsafe void MyWindow_Load() {
    Guid* Ptr = SilkMarshal.GuidPtrOf<ID2D1Factory>();
    SilkMarshal.ThrowHResult(D2D.GetApi().D2D1CreateFactory(FactoryType.SingleThreaded, Ptr,
        null, (void**)Factory.GetAddressOf()));

    // get our render target
    SilkMarshal.ThrowHResult(Factory.CreateHwndRenderTarget(new RenderTargetProperties(),
        new HwndRenderTargetProperties(MyWindow.Native.DXHandle.Value, new Vector2D<uint>(600, 800)), ref Target));
    D3Dcolorvalue Black = new(1, 1, 0, 1);
    ComPtr<ID2D1SolidColorBrush> SolidBrush = null;

    SilkMarshal.ThrowHResult(Target.CreateSolidColorBrush(Black, null, ref SolidBrush));
    Brush = new ComPtr<ID2D1Brush>(SolidBrush.AsComObject());

    Target.BeginDraw();
    Target.Clear(new D3Dcolorvalue(0, 1, 0, 1));
    ID2D1StrokeStyle Style = new();
    float x = MyWindow.Position.X;
    float y = MyWindow.Position.Y;
    Box2D<float> DrawBox = new(x, y, x + 100, y + 100);
    //Target.DrawRectangle(in DrawBox, ref Brush.Get(), 0f, null);
    SilkMarshal.ThrowHResult(Target.EndDraw((ulong*)null, (ulong*)null));
}

unsafe void MyWindow_Render(double deltaSeconds) {
    try {
        Target.BeginDraw();
        Target.Clear(new D3Dcolorvalue(0, 1, 0, 1));
        // ID2D1StrokeStyle Style = new();
        // float x = MyWindow.Position.X;
        // float y = MyWindow.Position.Y;
        // Box2D<float> DrawBox = new(x, y, x + 100, y + 100);
        // //Target.DrawRectangle(in DrawBox, ref Brush.Get(), 0f, null);
        SilkMarshal.ThrowHResult(Target.EndDraw((ulong*)null, (ulong*)null));
    }
    catch (Exception ex) {
        throw ex;
    }
}

MyWindow.Run();
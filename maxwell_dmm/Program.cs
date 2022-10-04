using System.IO.Ports;

var serial = new SerialPort();

serial.PortName = "COM3";
serial.BaudRate = 2400;
serial.Parity = Parity.None;
serial.StopBits = StopBits.One;
serial.DataBits = 8;
serial.RtsEnable = true;
serial.DtrEnable = true;
var buffer = new List<int>();
var endOfTheStream = 14;

void Process(List<int> data)
{
    Console.Write($"\n{(First)data[0]} ");
    Console.Write((data[1] & 0x8) == 0x8 ? "-" : "+");
    Console.Write(SevenSegmentToDecimal((SevenSegment)(data[2] << 4 | data[1] & 0x7)));
    Console.Write(((SevenSegment)data[3]).HasFlag(SevenSegment.decimalPoint) ? "." : "");
    Console.Write(SevenSegmentToDecimal((SevenSegment)(data[4] << 4 | data[3] & 0x7)));
    Console.Write(((SevenSegment)data[5]).HasFlag(SevenSegment.decimalPoint) ? "." : "");
    Console.Write(SevenSegmentToDecimal((SevenSegment)(data[6] << 4 | data[5] & 0x7)));
    Console.Write(((SevenSegment)data[7]).HasFlag(SevenSegment.decimalPoint) ? "." : "");
    Console.Write(SevenSegmentToDecimal((SevenSegment)(data[8] << 4 | data[7] & 0x7)));
    Console.Write(" ");
    Console.Write((Ax)data[9] != Ax.None ? (Ax)data[9] : "");
    Console.Write((Bx)data[10] != Bx.None ? (Bx)data[10] : "");
    Console.Write((Cx)data[11] != Cx.None ? (Cx)data[11] : "");
    Console.Write((Dx)data[12] != Dx.None ? (Dx)data[12] : "");
    Console.Write((Ex)(data[13] & 0x6) != Ex.None ? (Ex)(data[13] & 0x6) : "");


}


serial.DataReceived += (sender, eventArgs) =>
{

    //Console.WriteLine(Convert.ToString(((byte)(serial.ReadByte() << 4)) >> 4,2).PadLeft(8, '0'));
    //Console.WriteLine(Convert.ToString(serial.ReadByte() , 2).PadLeft(8, '0'));
    var b = serial.ReadByte();
    var index = b >> 4;
    var data = b & 0x0f;
    buffer.Add(data);
    if (endOfTheStream != index) return;
    Process(buffer);
    //buffer.ForEach(Console.Write);
    buffer.Clear();
    //Console.WriteLine($"{Convert.ToString(data , 2).PadLeft(8, '0')}:{index}");
};
serial.Open();
Console.ReadLine();

int SevenSegmentToDecimal(SevenSegment sevenSegment)
{
    var i = sevenSegment switch
    {
        SevenSegment.b | SevenSegment.c => 1,
        SevenSegment.a | SevenSegment.b | SevenSegment.g | SevenSegment.e | SevenSegment.d => 2,
        SevenSegment.a | SevenSegment.b | SevenSegment.c | SevenSegment.d | SevenSegment.g => 3,
        SevenSegment.f | SevenSegment.b | SevenSegment.g | SevenSegment.c => 4,
        SevenSegment.a | SevenSegment.f | SevenSegment.g | SevenSegment.c | SevenSegment.d => 5,
        SevenSegment.a | SevenSegment.f | SevenSegment.e | SevenSegment.d | SevenSegment.c | SevenSegment.g => 6,
        SevenSegment.a | SevenSegment.b | SevenSegment.c => 7,
        (SevenSegment)0xF7 => 8,
        SevenSegment.a | SevenSegment.b | SevenSegment.c | SevenSegment.f | SevenSegment.g | SevenSegment.d => 9,
        _ => 0
    };
    return i;
}

[Flags]
public enum First
{
    RS232 = 1 << 0,
    AUTD = 1 << 1,
    DC = 1 << 2,
    AC = 1 << 3
}

[Flags]
public enum SevenSegment
{
    None = 0,
    a = 1 << 0,
    f = 1 << 1,
    e = 1 << 2,
    decimalPoint = 1 << 3,
    b = 1 << 4,
    g = 1 << 5, 
    c = 1 << 6,
    d = 1 << 7
}

[Flags]
public enum Ax
{
    None = 0,
    Diode = 1 << 0,
    k = 1 << 1,
    n = 1 << 2,
    u = 1 << 3
}


[Flags]
public enum Bx
{
    None = 0,
    Beep = 1 << 0,
    M = 1 << 1,
    Percent = 1 << 2,
    m = 1 << 3
}

[Flags]
public enum Cx
{
    None = 0,
    Hold = 1 << 0,
    Rel = 1 << 1,
    Ohm = 1 << 2,
    F = 1 << 3
}

[Flags]
public enum Dx
{
    None = 0,
    Battery = 1 << 0,
    Hz = 1 << 1,
    V = 1 << 2,
    A = 1 << 3
}

[Flags]
public enum Ex
{
    None = 0,
    Empty = 1 << 0,
    Celsius = 1 << 1,
    mV = 1 << 2,
    Unknown = 1 << 3
}
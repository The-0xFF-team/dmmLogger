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
    var b = serial.ReadByte();
    var index = b >> 4;
    var data = b & 0x0f;
    buffer.Add(data);
    if (endOfTheStream != index) return;
    Process(buffer);
    buffer.Clear();
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
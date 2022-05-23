## Example
```c#
public static async Task Main() {
    var file = Path.Combine(Directory.GetCurrentDirectory(), "assets.csv");      
    var result = await SupercellDecoder.LoadAsync(file);
    var content = await result.ReadAsByteArrayAsync();
    await File.WriteAllBytesAsync("output.csv", content);  
}
```
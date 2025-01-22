using System;
using System.Diagnostics;
using System.IO;

class Program
{
    static int Main(string[] args) // 修改为返回int类型
    {
        // 检查是否提供了目录参数
        if (args.Length < 2) // 修改为检查至少有两个参数
        {
            Console.WriteLine("请提供要处理的目录路径作为命令行参数。 \nPlease provide the directory path you want to process as a command-line argument.");
            return 1; // 返回错误代码1
        }

        string directoryPath = args[0]; // 使用第二个参数作为待处理目录

        // 检查目录是否存在
        if (!Directory.Exists(directoryPath))
        {
            Console.WriteLine($"目录不存在: {directoryPath} \nThe directory does not exist: {directoryPath}");
            return 2; // 返回错误代码2
        }

        // 创建输出文件夹
        string outputDirectory = directoryPath + "OUTPUT";
        Directory.CreateDirectory(outputDirectory); // 创建输出目录

        // 获取所有.h264文件
        string[] files = Directory.GetFiles(directoryPath, "*.h264");

        // Console.WriteLine($"在目录 {directoryPath} 中找到的文件数量: {files.Length}");
        // foreach (var file in files)
        // {
        //     Console.WriteLine($"找到文件: {file}");
        // }       

        if (files.Length == 0)
        {
            Console.WriteLine("没有找到任何.h264文件。 \nNo .h264 files found.");
            return 3; // 返回错误代码3
        }

        // 创建或清空log.txt文件
        string logFilePath = Path.Combine(directoryPath, "log.txt");
        File.WriteAllText(logFilePath, "转换日志:\n\n");

        foreach (string file in files)
        {
            // 获取文件名和扩展名
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
            string outputFilePath = Path.Combine(outputDirectory, fileNameWithoutExtension + ".mp4"); // 输出到新目录

            // 创建转换命令
            string arguments = $"\"{file}\" \"{outputFilePath}\"";

            // 创建进程来运行MP4Convert.exe
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = @"MP4Convert.exe", // 请替换为MP4Convert.exe的路径
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = false
            };

            try
            {
                using (Process? process = Process.Start(processStartInfo))
                {
                    if (process != null) // 检查process是否为null
                    {
                        // 输出转换过程中的信息
                        string output = process.StandardOutput.ReadToEnd();
                        string error = process.StandardError.ReadToEnd();
                        process.WaitForExit();

                        // 将输出信息写入log.txt
                        File.AppendAllText(logFilePath, $"正在转换 {file} 到 {outputFilePath}...\n");
                        if (!string.IsNullOrEmpty(output))
                        {
                            File.AppendAllText(logFilePath, output + "\n");
                        }
                        if (!string.IsNullOrEmpty(error))
                        {
                            File.AppendAllText(logFilePath, "错误: " + error + "\n");
                        }
                    }
                    else
                    {
                        File.AppendAllText(logFilePath, $"无法启动进程: {processStartInfo.FileName}\n");
                        return 4; // 返回错误代码4
                    }
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(logFilePath, $"处理文件 {file} 时发生错误: {ex.Message}\n");
                return 5; // 返回错误代码5
            }
        }

        Console.WriteLine("所有转换完成. 日志已保存至 log.txt。 \nAll conversions are done. The log has been saved to log.txt.");
        return 0; // 返回成功代码0
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

using CNTK;

namespace IOT_Project
{
    public class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length == 2)
            {
                List<ValPerc> output = EvaluationSingleImage(DeviceDescriptor.UseDefaultDevice(), args[1]);

                ValPerc actualVal = null;
                double maxperc = 0;
                foreach (var item in output)
                {
                    if (Convert.ToDouble(item.percent) > maxperc)
                    {
                        maxperc = (Convert.ToDouble(item.percent));
                        actualVal = item;
                    }
                }
                if (actualVal != null)
                {
                    OutputFile(args[0], actualVal.val);
                }
                else
                {
                    OutputFile(args[0], "invalid");
                }

            }
            else
            {
                OutputFile(args[0], "invalid params");
            }

        }
        public static void OutputFile(String filename, String message)
        {
            using (StreamWriter outputFile = new StreamWriter(filename))
            {
                outputFile.WriteLine(message);
            }
        }
        public static List<ValPerc> EvaluationSingleImage(DeviceDescriptor device, String fileName)
        {
            const string outputName = "z";
            var inputDataMap = new Dictionary<Variable, Value>();

            // Load the model.
            Function modelFunc = Function.LoadModel(@"C:\Users\Rohan\Downloads\CNTK_HandsOn_KDD2016\ImageHandsOn\Models\cifar10.cmf", device);

            // Get output variable based on name
            Variable outputVar = modelFunc.Outputs.Where(variable => string.Equals(variable.Name, outputName)).Single();

            // Get input variable. The model has only one single input.
            // The same way described above for output variable can be used here to get input variable by name.
            Variable inputVar = modelFunc.Arguments.First();
            var outputDataMap = new Dictionary<Variable, Value>();
            Value inputVal, outputVal;
            List<List<float>> outputBuffer;

            // Get shape data for the input variable
            NDShape inputShape = inputVar.Shape;
            int imageWidth = (int)inputShape[0];
            int imageHeight = (int)inputShape[1];
            int imageChannels = (int)inputShape[2];
            int imageSize = (int)inputShape.TotalSize;

            Console.WriteLine("Evaluate single image");

            // Image preprocessing to match input requirements of the model.
            Bitmap bmp = new Bitmap(Bitmap.FromFile(fileName));
            var resized = bmp.Resize((int)imageWidth, (int)imageHeight, true);
            List<float> resizedCHW = resized.ParallelExtractCHW();

            // Create input data map
            inputVal = Value.CreateBatch(inputVar.Shape, resizedCHW, device);
            inputDataMap.Add(inputVar, inputVal);

            // Create ouput data map. Using null as Value to indicate using system allocated memory.
            // Alternatively, create a Value object and add it to the data map.
            outputDataMap.Add(outputVar, null);

            // Start evaluation on the device
            modelFunc.Evaluate(inputDataMap, outputDataMap, device);

            // Get evaluate result as dense output
            outputBuffer = new List<List<float>>();
            outputVal = outputDataMap[outputVar];
            outputVal.CopyVariableValueTo(outputVar, outputBuffer);
            var s = PrintOutput((uint)outputVar.Shape.TotalSize, outputBuffer);
            return s;

        }
        private static List<ValPerc> PrintOutput<T>(uint sampleSize, List<List<T>> outputBuffer)
        {
            Console.WriteLine("The number of sequences in the batch: " + outputBuffer.Count);
            int seqNo = 0;
            uint outputSampleSize = sampleSize;

            System.Collections.Generic.List<ValPerc> listOfOutputs = new System.Collections.Generic.List<ValPerc>();


            foreach (var seq in outputBuffer)
            {
                Console.WriteLine(String.Format("Sequence {0} contains {1} samples.", seqNo++, seq.Count / outputSampleSize));
                uint i = 0;
                uint sampleNo = 0;

                uint cnt = 0;
                foreach (var element in seq)
                {

                    if (i++ % outputSampleSize == 0)
                    {
                        Console.Write(String.Format("    sample {0}: ", sampleNo));
                    }

                    String val = "";
                    switch (cnt)
                    {
                        case 0:
                            //  val = "airplane";
                            val = "closed";
                            break;
                        case 1:
                            //  val = "automobile";
                            val = "open";
                            break;
                       
                    }

                    listOfOutputs.Add(new ValPerc { percent = element.ToString(), val = val });
                    Console.WriteLine("------------" + val);
                    Console.Write(element);
                    if (i % outputSampleSize == 0)
                    {
                        Console.WriteLine(".");
                        sampleNo++;
                    }
                    else
                    {
                        Console.Write(",");
                    }
                    cnt += 1;
                }

            }
            return listOfOutputs;



        }
    }
}

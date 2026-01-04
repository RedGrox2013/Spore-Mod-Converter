using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Spore_Mod_Converter
{
    internal class PackageConverter : IDisposable
    {
        private readonly Stream _package;

        private const uint DBPF = 0x46504244;

        public PackageConverter(string packagePath)
        {
            _package = File.OpenRead(packagePath);
        }

        public async Task ToPrototype2008PackageAsync(string outputPath, CancellationToken cancellationToken = default)
        {
            _package.Position = 0;
            byte[] buffer = new byte[sizeof(uint)];
            _package.Read(buffer, 0, buffer.Length);
            if (BitConverter.ToUInt32(buffer, 0) != DBPF)
                throw new FileFormatException(new Uri(outputPath, UriKind.RelativeOrAbsolute), "File is not DBPF");

            _package.Position = 0;
            using (var output = File.Create(outputPath))
            {
                await _package.CopyToAsync(output, 81920, cancellationToken);

                using (var writer = new BinaryWriter(output))
                {
                    writer.Seek(sizeof(uint), SeekOrigin.Begin);
                    writer.Write(2);
                }
            }
        }

        #region IDisposable realisation
        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: освободить управляемое состояние (управляемые объекты)
                    _package.Dispose();
                }

                // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить метод завершения
                // TODO: установить значение NULL для больших полей
                _disposedValue = true;
            }
        }

        // // TODO: переопределить метод завершения, только если "Dispose(bool disposing)" содержит код для освобождения неуправляемых ресурсов
        // ~PackageConverter()
        // {
        //     // Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}

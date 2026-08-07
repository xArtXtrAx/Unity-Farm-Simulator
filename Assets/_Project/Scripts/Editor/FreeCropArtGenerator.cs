using System;
using System.IO;
using System.IO.Compression;
using UnityEditor;
using UnityEngine;

namespace FarmSimulator.Editor
{
    /// <summary>
    /// Reconstructs the approved free crop artwork as grid-aligned Sprite assets.
    /// Crop growth remains entity-based through SpriteRenderer, not Tilemap terrain.
    /// </summary>
    public static class FreeCropArtGenerator
    {
        public const string OutputRoot = "Assets/_Project/Art/Placeholder/Crops";
        private const int Size = 32;
        private const float PixelsPerUnit = 32f;

        private static readonly EncodedSprite[] Sprites =
        {
            new EncodedSprite("turnip", 0, "PVFLSkNBEHzdPf8PGYQggoqgEJCsspRsFFei4MILuHXvBTxDjuPdYnWP+mDezJuq7qqu9314ObzefD2cf96dfezW79uTt814uu7PV+3xsu5P024dt10uGt8v+hzHcdwO6TJoRY0qdeyD+gqLKxXKFClLxR7IU2A99woUXB+dF2YScsAybkoa6BKIibA8JXTw9MvhQOIY3ETNaf8BNAEBjD3yYgoet44W1BfFsALQavxORavY4ZbAbpBZTDupQ1lshmJulZPnNJRYfWXUJYY/6GV0aaZJ5iKji4ORDLZViL7Dv4NCVStp8hvrXM6c80StqkKnmjbGsFymv2Dfi3GnQjNPkTUXz5PvRJMiU7TsLKEw39bFuyTe8giMFf84ybKpkhRj/DPJHHGKeuaZX5scp/llnQH84pAWV6/+W4F3xv9fdW6b8QM="),
            new EncodedSprite("turnip", 1, "PVG7SgRBENzqnvfOcoMnCyqCF4iBIJopIkYGBuYGfoC5YGbkv/ibZ/XscgdzPd1V1V09+/1Xfz/Ofl5Pvu7nz7vt+9Xm+Tq97erLeXk6zQ9zuj0Ol0d6UfVxsN++7dtNwwaTNgQMEmZGGQDWgg4I26bTcnMDKqt10zBhFI+E3Lk8EuARmVeMvqEKOdJ5rDqBKPHRBWYigFi2Mgq8NKmgClEMdWR1HOyNqhFKlZORnJGIUOeJmsKpdc6sV+LmZZQJWYWaQkZhr0S+OTN+Ym7zHSuGKd0J3SVm2pHutHPpncqEZS5xErR3cIyx3zzUAf7AYnKYVsTJsq32f9ggi+tbTVRRb/x+vHIbatRJ92WswhfP6w5FbEJG8tGOUC1x7bVMJxbMeVFu57IU25L3tHLsxYnGppXTeaSGyTHaN9u1fw=="),
            new EncodedSprite("turnip", 2, "PVE5TgQxEJwut8fHjBmDgACR7EpsSLYZCRIJCWSIABHxAL6AxCN4AO/gFfwHqu3RBr6qq6rL9tdvevzU75er16ejj9uL9/3p2/XJ7kaed0cPm/nuMu3Pxu252xZ3P8gwDH/1pyIcV1kkyJwqZ5HBVifcV5lFMBALsdouGlN0QFBDl3BQgKgEDCCXJw6ZEVgMU5XC04JCtwABO7lSzXuSJBMyClWLFJlckkx09q2q1s04XlVG7hiK8wjv1JRSXOWcZeRarELcY2w19jItiApRzr55rExRZ+5ZIjXRvDj8yjRVaZ0VIMc0yRy0NqY5jrCk6nhx6nPv5PzaIbfUxvZ0UEctIlLDI7VYO2VUYwpQqY/iQ/dL8FCFIax3R6ZDT2/9YksBpgf6Okrk3Q3viTOivQ/fl305rMqeMPd2bszSXwRZ2xqZUPMhKW/V35g/UjAl/iEsQekvZOim/gM="),
            new EncodedSprite("turnip", 3, "RZJBThwxEEXnV5Xt7na72wghpIgFiZQgISQOkOySHRt2rLKIxIoTcIUcIRcJC27AYbhC8qt6GEbqKfvX8//l6Xl6/fD88u3hr3z/jT93n3/eL9f3uy83ePx6+uvq6PbT/ONsvLjUi67nZcfPvz52zMiiWKzretyRIbqDoAg7qtyvQ0fhk831DO4gXCvm7msNnjs+Ekw52nRXtxoq1taxomioEt2kmjGn4M1nUXeQDE8u4t8kJc6r9+lFLqcdIc6a3jpl3ynJfSUyeDb7xNtJ0ub5sxZzf9mIyMlSYooZJXcsXHvPCfehqjN/gYXPKrOse33fI9+wrF0aKiap3HlaQdKqE7VWu1QZbcCIiaqBqzSgGs+o51V412TMIMOdDlEXeHfCoFt1PRzBK9ClxZreVBtZntyz1WkSnEdaAiwIquHgvk2Y794S84RucHJQcmYxAblDfiVnOvj8kTjKkN+pQ7L3hD2Y0dur+8shN+7hZLMqfpspjXEr6rbdLqbj+2gnlf/DRVruqXmVRQ+V07eP/T8="),
            new EncodedSprite("turnip", 4, "XZI9bhRBEIWnqn+nf2aaaCVAQrIEdrCZEzv1GRABlyBCJEhsigT3QByDkKtYOHbIezW7lu2Verv71Vev30zP/d2P37cv/v39cvgTv/3aHb6/ufkqPz+8+3S1e/922X+Us2u5fJXOu5vChN/FkCrOiTgp4vHPVTQl9iGrLFq23bHuJSopj761DllQMV2jo0fkyhTrwLrkIV1KENvRoVBVfzzHGQGWBHTn2WdJ1JSZGeHpnfVHO8Whn3Xzk1qYpGqxs5mUlLGOZ9U6OLtquxhI1ghfNW9HZ7iEqOAt2anaqYLBzF5pWsnYiXw7VbvkOaG+YrWuQ5qkopJkXocu2GXVOHmVWboulY6zZIykmicl2WR2LQwSYnVNWz1OorIxs2NP8yeKvumR+1OikVC40kWnaHXSmJ9QetKa5UpBTwT8nrPI7TeNPQHZshrpn5GWC5RLmo9PCyWkh4Tt+KQtzZoDRtz0rkN7gCNyQans69T9qKB9h26jgsONb3cAL7574fvm6J+Hrq/xTXnc08sh0eP73Y//"),
            new EncodedSprite("potato", 0, "JVK7TsRADLyx95nN6lYnQAKKE6IIFQ1UCIkCiRJdxwdAS8sPUPMF/BB/dYydSMna6/F4bOfv9/BzuP5+vvx6PP+8P32/3b0t4/Wqv+zb00V9OMt3J+lmF/eTfGzsOY7jWAY6BrYyi5+Y0WSCeVv0PjBpRkTgm1BAW4NEKSAG22rZM6KIBCiCqMh6QgC+llcxl8FchhyXYFFlJNM2XmXE/IKug7UHc5Lh+a2MbngGx1fayrtIniiJWolH9ozoKOG364rybNqdapv7usZhWUlMfRXnpV9YL0ujVb1ycX3WBzuiN8PwjQqr2g2nQDSoqNo9EZxBgOtoxDZMpov8vGWLcO3UmODnylhdR5Gg1hPBWGdoeM6PPRpLYTVjTNZ1yJrE9xGzrCptDpPPgUpCkRoK92axwgpFstdrPltDkEuqcNNSyV9lUtu6zaihT8TY/5DnyO1qX8Y/"),
            new EncodedSprite("potato", 1, "NZE9TgQxDIXn2ZP/CZsVaFcCCbQdogChLelooAKJC9DQ7A3o6LgC9+CC8JzASNbY488vb+LP79XH2+nXy+7wePJ+tz3cHr9etafL+rxbHi7K/Vnab/z12p0fyX6y56f9tJuGlTCgAHRrGTAxClQn1E1D0EkgE1TquqFiQRZ2+SVwapIiiVlCdZz2RhTWDp4RkXulEhBlhmMUVG1USTy3WpCZVTBzIlmXqgELVZ0INYpUGBUgpNlhVeBmIZ/JDd58FVOSoTQoz/p/ptKNY4+uJI/8jzWPiZX5LX0ukbE5q0x3OPGS+j94id2xI5c7H7u3MZ3EE59hwRnmsVOZlVGZkdQrCT/+25QHE3tORQulCzuhv32IMpzaCebObpF3HbMxc2fGHmTsIIemi9ZgN7RwY7afZdd+AQ=="),
            new EncodedSprite("potato", 2, "TVK7SgRBENzu3tl5LrtocAaeoEYmZmIkGImJIOaCgYE/4RccmPoX/pHfolU9J3gw16/q6pqe/fpeHj/l4/X48kV299v3m6O3q4Oz2+H5Ynk6b3fbfL2ZTjd2MuvDwN/PultlCTgSxXCWvErDMRXETRHBGUQkRmRF0r+MIBPoLRNrQ6+NzPxZ81oUAWcM7LHRmYGNxeeqOQ5WMf9wNVrPjAZEi2StVASEjBGqZs9G1920aNUm3oNKlaxJCuyM+gwW/mcJOmqQxLzf1XMQM3Ws9SnFiK5Ei0oAphiyI44kYzwLEQV9wbqXgO2Vhiz69nGFmqyTz6RS+EZLriB9coKHS0EZcdmKuUbsnagJ2oFVRbVOmQhFbIyzo6hDjejUZ0ufPTljcOauSDXveYFER7HkzL1HuR3fRrauFzfTROYRc5XZybJPSW6LzGTT7Putc+GuXDVtG/0GfAG8zoz3wXbC3nLf+Crq6foL"),
            new EncodedSprite("potato", 3, "PVE7jhVBDJyy3d/p4TWsIGAlJILNIEEi2nivwAUId0XCHbgBB0HiMByBiDNQds/boNs9rnK57Pn17+XPP/nbb3n6fvn4dfvxcPv46ebL3Yu7B7z9gPvb+vkmv7vIdmzb9prn70QVQ7WJAxV8ob2aUEA2ACaeN4gwwdgu5EPMsYrJDLAFV1BnYFEXXN7QUEE7qKPUN4kez4rCfjjMK81cTxTBUBE1VyDTmFP6nOQqNvW4PGg2IR5V7qHBsUrAfavnQ4d6iSwLL2smdmtE1I3g6rm6I/FsbGIPNddnN6FXiWkX0103WKL3dJ1HvZNuvr7YaYs+7KYnKnZu2VZf8d7G3TirrQo9O4hJs7Wxo04Zcv4fHb4nzuGzDH5X7BjqjMlXl4yCrnx53ivibwzHUKQrUe5p8M6s4K1Je6AZKapdZZduOZh75D0Wzdi1l/Tcocji8OVqsriFsbjuVW2pa0I+exNfiikVMl3PK1KK95TlLaZpVKlLKSpOhNF49rTi6fSK9sRJebL07NrDUSnc82gxG/bKyE70wr33N4P3eD//Aw=="),
            new EncodedSprite("potato", 4, "VZBPahZBEMW7qrqn/01PJugmICIIunXlQslCPIWusg7uBYXcIIfIFbyDF8hBPIPvVXcEPxhmvqpXv3r1fv15fv+Yvv2Wmwf9+f3Z5x/y/ja8/hruPl19eXvx6lo+vigfjvhyaOgBvzenbCIxqJlIwreKaBCRnE5peXXxfxMI0PW3sZJkxKk5pIlxSg1VW5wAFL/bOLVJlqQkBFHdJBk7GXOHXDRQvLKhAr6K9yZnEhWz20YdtyV3ZFNNJyTSt0KISlP41qke9OY98ptPcE+aV/jWf3rjTEYWT/6Sqk4fWb1LD8a9qgHYDd8t4vEbtqWdCcKV4a7I1OCp0MvTVXml6fkwB8UuniCtUsdJd6rZXdNJZrxQJLJsyG7dhl/SyFwZkz3gssupQ6oWqdZlZw4KVRrSraJ7IJuurkOvSpEILXU+46lgh3bMF63Omuq+FKQWi/6NR3evTLozlx41rbZLLSX2WFaXuyvf6NdYIhiRTotO4r78VRIUs1q36TEqN4zl/pA9+v5Bxv+boWnoZXInkRfREbNxX5eeDWoHMhu2X2Lm8bT27vwL"),
            new EncodedSprite("potato", 5, "PZI9bhwxDIWXj/obabQjwEDcJCmCDdy7CNK6dGmkSe0TuHOT1r1T+jL2EVLkJjlBHqkdDzA/pL7he6T0+u/05+/t45u+PF/9fJGbX/L048vD98u7U7++l6938umb3FykzxsO4cDrNOQoMRwEskgEeAdZAFiGX3JcnNAZ84355lMXabJxvZGDTAJGWg0lRYYVpZHZWJXRuS7VNO60qeQhnUTEghrNQWU2BGOrHHVIxZgOEcwlv42Ivt5itKcc485RKZwdMx89wzjQcR6eAaKpT3/vTtyT1+zkTKFhduHdTEWFq0fPUdfchYGGqlPL5hFgjiqaO+uc4RbI0WHz3mxlJ41b5uQTGbVZVO9zKsSd07j7INd1zswcR5hGmDOhkx54U2tl1qiCzLvD4nbuZJUia+jKHJ3xm2RBIqnCcUh2gpOQrIlREoU9symjGO3ZHIV/FKdWz3FAzBhZzuwmqxZyrBFszXxkpzJXV3ZiakP9fyjIJaEbixPMvTHTI3OavB/1iuaBhDufFHtGiabO1ZD8v+m86K63T2ZdGJeptMJnioFe6Td7TvoyLvtvnmDuW/84uA92UuqHwfPPffKTE3yPbWfb1fgP"),
            new EncodedSprite("radish", 0, "RVI7TsQwFMx7z45/MTEIgaBAWoS0WtFQIFFRUNCxDRIN9ByAC1BzBC7EtZYZZxGRnDzbM+N54/x8b7+2q8+H84+70/eb47fN4cu6PV8dPK3q48V0f5ZuT8L1kb+c7HXgs2u7tmkyyyTFVymSpWjFrMpcmxZJEmQUjxE0yqimDnUELhGVGt4Z+ypODMOLM8PMWOEryjoSGxv0nROhgpl2fOpqmBClSao1qdqg+bea4GgwcpINmC3aubvK8EnV2Bluf6bHqtMBOKKgBF6BNnHSd8nxuniPnUWlIrDL8yRr6ki6C9BiBwVoeJNMh+yYzWGf6wXpDGCzTsAve6Fr8ZTS/QWw0IgsiQgYTI1Ycjmcjp1RkHQD26tTT54HCHmQueQGpgbellB53Cfml7sy+vbK2uHWcIPARh17F/+J1aV/rqPfiHwxgAkWOwPd9IyKw9uylpBdsdy7qrn1/2R2dZwMldV1+wU="),
            new EncodedSprite("radish", 1, "PZA9TgNBDIX32fO7s6MMkUgoIkGkVBTQ0CBR0CGq5BIRJ+AElHRchCOG59mElXZ3/Pz5eezv36uv4+Znf/fxtvx8Xh8fl/v76bCt77fldZNfbtLTddit3K7qYbDn1E7toclCGiZRhHVDBjBAMUF1wLRq9heVgfq0bFhoRYDF/JIMbiS7SA1VmcWIqB4RiU6h+xRE5+D5n4nMbtU6UnEqsFzkeer0KJ7KrCarAWmehBWhR/R1Ai9FqgSpOvLsSFiukDHyHElkJrBiZGbs3RhJobd1yGcqibkWzHfycumeEb2XyKr5Luafu3+kW+Mb4Lx4J8TIhM4kc/93zH26PpNzyZyjzFwUL+08YVHrloR78xEhxIvOCeZtVRRvc2RNGtXIEYtZR47NtsHb117HWdW2Tc2T2LY/"),
            new EncodedSprite("radish", 2, "LVE7chUxEHw9I2lX0q5XVU6gKOyyI07gcmQHDhw4gAM4oQi4BRmJMwIOx2lM9+htrT7T090zkn7/qy9v+PPz6u4H/n69/vX48fv9uH3C65eLb7fb8+d2/2G5+eTXmz2c9L2Pt2FHHjh8wbFyxVIHFgMcmymGn8CpaO84kbMkIQs2IGu35eBFjtg5S51pPahkVGPGyYnlUIvdOMwN2cW8VGU38RR76Dcpw9ldv+ouacO+jOhqt27NNvHVC+OGais6dva1k6u5o3hChvDDnMjKohNp6D6IV6xJa0YytsS4m5T09C5PE7ZDp22WfPrSx1L47mjT0aRrxOUtdpWna9eJFIjVYq9a2QoZiospU6aK55j9FFgoaiJiNUbWpal3G+GTbNi5F7NEz1AkMAh1Ya5CHUf9OO/KeivzxXKWgl0knUOsKn6csHk1DbLPva0zy9rzNvbooM57PojyPfrU5mF76ZVvFK/Bm7wZ/wE="),
            new EncodedSprite("radish", 3, "TZHNTQQxDIXn2fmdDLMRcIALCHEFOqADTlTABYkCONICohJOlEIZFAHPzs6KlbJxnC/Pz57Pn/7xnZ+/8PiO15fj26fp7f7s6bY/XLare9xcxLvTeF0w6fmUJ/5++9xRJWIXO3aynnQoIBMEVTuiCs+1k4EGz4JZgLFi7R5jEj5htDoD6Noxk4y2BCIeKRkjqUdyF0wzRldWy7IWfahp2Q6rHKWTtXp9U8+8Ed6YLKxaxH+G+c1RpUpw/65So/+zjg4Gw/XepyqrzlibzcFyYw7DbzzwVrm6d03W5Xg3PFV3P6l5WtX7SzYhZoSUVtXgeurzoZ+F3lxT7RtwWWVxJ6xhM2pSJKPYTnp2NzaXWZbQsLCzhqLWWdHE4RRtyFJ5V4RnewlnkEjtnDK1hVQYRNhqGJcHx2qM/PXClRAC2TiyG8tT2Gsj7PMt0YP42TVXkuTUeuUuScKmlo3KctA6MEVzS4N2B+u/+5bp9Wh0Zc6KTdG6lpJsRk3ds/ht6tIi93mxjul1uep/"),
            new EncodedSprite("radish", 4, "TVGxjtUwEPTO2rFjx7zoKO4ampNAAtHQILpraK+830Cn+wwqWv6Chorf4Bdo+QZmNpG4womzOzM7s/n599X33/uXX/nx29X9V/v0lH7cvX36eP3w5sWHB7u9T+/flc/X6+vdk92klP7stlo2QzLjGzzdVmRWi5knqDrabsNyYA4cOQBRxbKTzFtfhSkGCkNciItAAHzy1k+lTraRQ7Ucapnfp/KgTreC8OKUVj/4dGNS7E7PPGMp9CEs57v8imORYbUhFPZzlqz8VypRPXDSGHbJREK7CGiWd9cmlOjcTjl99VN5xuZWoQmmLz0snQ5KTCh1t4tzmnmOXcR8Vwfdy6GVdQZiGjoyeMdQNoQvnou8EL/mHjMne8WVdy27VNALpI6jP7B6eIj/MrVhMuaZVxufGHlSeWNXiA2tbAhdG5VfYIcpGxFEodpiFc0qK7Uu1vJmzSs7B+bC01AxfWtCSmvzpj5ZnEB8aBBXlyV0NLtxX0voSklVYslz7nyRLz9ZmHRUkZ7NpK9cwa4v9KfNSy8qONw+Q7a21HBIbD58RXJspTnz5i0zMycr9eR/mS+VcDvueb+Zt/s/")
        };

        [MenuItem(
            "Tools/Farm Simulator/Farm Development Kit/Free Placeholder Art/" +
            "Generate Crop Growth Sprites")]
        public static void Generate()
        {
            EnsureFolder("Assets/_Project/Art/Placeholder");
            EnsureFolder(OutputRoot);

            foreach (EncodedSprite encoded in Sprites)
            {
                string path = $"{OutputRoot}/{encoded.CropId}_stage_{encoded.Stage}.png";
                File.WriteAllBytes(path, DecodePng(encoded.Payload));
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
                ConfigureSprite(path);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log(
                $"Generated {Sprites.Length} free crop sprites at {OutputRoot}. " +
                "Each sprite is 32x32 px, 32 PPU, bottom-center pivot, and fits one 1x1 grid cell.");
        }

        private static byte[] DecodePng(string payload)
        {
            byte[] compressed = Convert.FromBase64String(payload);
            byte[] raw;
            using (var input = new MemoryStream(compressed))
            using (var deflate = new DeflateStream(input, CompressionMode.Decompress))
            using (var output = new MemoryStream())
            {
                deflate.CopyTo(output);
                raw = output.ToArray();
            }

            if (raw.Length < 48)
            {
                throw new InvalidDataException("Crop sprite payload is missing its palette.");
            }

            var texture = new Texture2D(Size, Size, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Point;
            var pixels = new Color32[Size * Size];

            int source = 48;
            int target = 0;
            while (source + 1 < raw.Length && target < pixels.Length)
            {
                int count = raw[source++];
                int paletteIndex = raw[source++];
                Color32 color = paletteIndex == 16
                    ? new Color32(0, 0, 0, 0)
                    : new Color32(
                        raw[paletteIndex * 3],
                        raw[paletteIndex * 3 + 1],
                        raw[paletteIndex * 3 + 2],
                        255);

                for (int index = 0; index < count && target < pixels.Length; index++)
                {
                    pixels[target++] = color;
                }
            }

            if (target != pixels.Length)
            {
                UnityEngine.Object.DestroyImmediate(texture);
                throw new InvalidDataException(
                    $"Crop sprite decoded {target} pixels instead of {pixels.Length}.");
            }

            texture.SetPixels32(pixels);
            texture.Apply(false, false);
            byte[] png = texture.EncodeToPNG();
            UnityEngine.Object.DestroyImmediate(texture);
            return png;
        }

        private static void ConfigureSprite(string path)
        {
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null)
            {
                throw new InvalidOperationException($"Could not load TextureImporter for '{path}'.");
            }

            var settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            settings.textureType = TextureImporterType.Sprite;
            settings.spriteMode = (int)SpriteImportMode.Single;
            settings.spritePixelsPerUnit = PixelsPerUnit;
            settings.spriteAlignment = (int)SpriteAlignment.Custom;
            settings.spritePivot = new Vector2(0.5f, 0f);
            settings.filterMode = FilterMode.Point;
            settings.mipmapEnabled = false;
            settings.wrapMode = TextureWrapMode.Clamp;
            settings.alphaIsTransparency = true;
            importer.SetTextureSettings(settings);
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
        }

        private static void EnsureFolder(string path)
        {
            string[] parts = path.Split('/');
            string current = parts[0];
            for (int index = 1; index < parts.Length; index++)
            {
                string next = current + "/" + parts[index];
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, parts[index]);
                }

                current = next;
            }
        }

        private sealed class EncodedSprite
        {
            public EncodedSprite(string cropId, int stage, string payload)
            {
                CropId = cropId;
                Stage = stage;
                Payload = payload;
            }

            public string CropId { get; }
            public int Stage { get; }
            public string Payload { get; }
        }
    }
}

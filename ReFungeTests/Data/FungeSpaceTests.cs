using System.Text;
using ReFunge.Data;
using ReFunge.Data.Values;

namespace ReFungeTests.Data
{
    [TestFixture]
    internal class FungeSpaceTests
    {
        private FungeSpace _space1D;
        private FungeSpace _space2D;
        private FungeSpace _space3D;

        [SetUp]
        public void SetupEachTest()
        {
            _space1D = new FungeSpace(1);
            _space2D = new FungeSpace(2);
            _space3D = new FungeSpace(3);
        }

        public class FungeSpaceIndexingTests : FungeSpaceTests
        {
            [Test]
            public void Indexer_ReturnsCorrectValue_AfterSet()
            {
                FungeVector vector = new(1, 2, 3);
                FungeInt value = 42;
                _space3D[vector] = value;
                Assert.That(_space3D[vector], Is.EqualTo(value));
            }
            
            [Test]
            public void Indexer_UpdatesValue_WhenSettingTwice()
            {
                FungeVector vector = new(1, 2, 3);
                FungeInt value = 42;
                FungeInt newValue = 43;
                _space3D[vector] = value;
                _space3D[vector] = newValue;
                Assert.That(_space3D[vector], Is.EqualTo(newValue));
            }

            [Test]
            public void Indexer_TreatsTrailingZeroesAsEqual()
            {
                FungeVector vector1 = new(1);
                FungeVector vector2 = new(1, 0, 0);
                FungeInt value = 42;
                _space1D[vector1] = value;
                Assert.That(_space1D[vector2], Is.EqualTo(value));
            }

            [Test]
            public void Indexer_SettingUpdatesBounds_WhenAddingCells()
            {
                FungeVector vector1 = new(1, 2, 3);
                FungeVector vector2 = new(4, 5, 6);
                FungeInt value = 42;
                _space3D[vector1] = value;
                _space3D[vector2] = value;
                Assert.Multiple(() =>
                {
                    Assert.That(_space3D.MinBounds[0], Is.EqualTo(1));
                    Assert.That(_space3D.MinBounds[1], Is.EqualTo(2));
                    Assert.That(_space3D.MinBounds[2], Is.EqualTo(3));
                    Assert.That(_space3D.MaxBounds[0], Is.EqualTo(4));
                    Assert.That(_space3D.MaxBounds[1], Is.EqualTo(5));
                    Assert.That(_space3D.MaxBounds[2], Is.EqualTo(6));
                });
            }

            [Test]
            public void Indexer_ReturnsSpaceCharacter_WhenCellIsEmpty()
            {
                FungeVector vector = new(1, 2, 3);
                Assert.That(_space3D[vector], Is.EqualTo(new FungeInt(' ')));
            }

            [Test]
            public void Indexer_ThrowsException_WhenVectorIsTooLarge_Get()
            {
                FungeVector vector = new(1, 2, 3, 4);
                Assert.That(() => _space3D[vector], Throws.ArgumentException);

                vector = new(1, 2, 3);
                Assert.That(() => _space2D[vector], Throws.ArgumentException);

                vector = new(1, 2);
                Assert.That(() => _space1D[vector], Throws.ArgumentException);
            }
            
            [Test]
            public void Indexer_ThrowsException_WhenVectorIsTooLarge_Set()
            {
                FungeVector vector = new(1, 2, 3, 4);
                Assert.That(() => _space3D[vector] = 42, Throws.ArgumentException);

                vector = new(1, 2, 3);
                Assert.That(() => _space2D[vector] = 42, Throws.ArgumentException);

                vector = new(1, 2);
                Assert.That(() => _space1D[vector] = 42, Throws.ArgumentException);
            }

            [Test]
            public void Indexer_DoesNotThrowException_DueToTrailingZeroes_Get()
            {
                FungeVector vector = new(1, 2, 3, 0, 0);
                Assert.That(() => _space3D[vector], Throws.Nothing);
            }
            
            [Test]
            public void Indexer_DoesNotThrowException_DueToTrailingZeroes_Set()
            {
                FungeVector vector = new(1, 2, 3, 0, 0);
                Assert.That(() => _space3D[vector] = 42, Throws.Nothing);
            }

            [Test]
            public void Indexer_HandlesNegativeCoordinates()
            {
                FungeVector vector = new(-1, -2, -3);
                FungeInt value = 42;
                _space3D[vector] = value;
                Assert.That(_space3D[vector], Is.EqualTo(value));
            }

            [Test]
            public void Indexer_SettingUpdatesBounds_WhenAddingNegativeCells()
            {
                FungeVector vector1 = new(-1, -2, -3);
                FungeVector vector2 = new(-4, -5, -6);
                FungeInt value = 42;
                _space3D[vector1] = value;
                _space3D[vector2] = value;
                FungeVector MinBounds = _space3D.MinBounds;
                FungeVector MaxBounds = _space3D.MaxBounds;
                Assert.Multiple(() =>
                {
                    Assert.That(MinBounds, Is.EqualTo(vector2));
                    Assert.That(MaxBounds, Is.EqualTo(vector1));
                });
            }

            [Test]
            public void Indexer_ShrinksMinimumBounds_WhenWritingSpaceCharacters()
            {
                FungeVector vector1 = new(1, 2, 3);
                FungeVector vector2 = new(4, 5, 6);
                FungeInt value = 42;
                _space3D[vector1] = value;
                _space3D[vector2] = value;
                _space3D[vector1] = new FungeInt(' ');
                FungeVector MinBounds = _space3D.MinBounds;
                FungeVector MaxBounds = _space3D.MaxBounds;
                Assert.Multiple(() =>
                {
                    Assert.That(MinBounds, Is.EqualTo(vector2));
                    Assert.That(MaxBounds, Is.EqualTo(vector2));
                });
            }
            
            [Test]
            public void Indexer_ShrinksMaximumBounds_WhenWritingSpaceCharacters()
            {
                FungeVector vector1 = new(1, 2, 3);
                FungeVector vector2 = new(4, 5, 6);
                FungeInt value = 42;
                _space3D[vector1] = value;
                _space3D[vector2] = value;
                _space3D[vector2] = new FungeInt(' ');
                FungeVector MinBounds = _space3D.MinBounds;
                FungeVector MaxBounds = _space3D.MaxBounds;
                Assert.Multiple(() =>
                {
                    Assert.That(MinBounds, Is.EqualTo(vector1));
                    Assert.That(MaxBounds, Is.EqualTo(vector1));
                });
            }

            [Test]
            public void Indexer_ResetsBounds_WhenEmptyingSpace()
            {
                FungeVector vector = new(1, 2, 3);
                FungeVector negativeVector = new(-1, -2, -3);
                _space3D[vector] = 42;
                _space3D[vector] = ' ';
                _space3D[negativeVector] = 42;
                FungeVector MinBounds = _space3D.MinBounds;
                FungeVector MaxBounds = _space3D.MaxBounds;
                Assert.Multiple(() =>
                {
                    Assert.That(MinBounds, Is.EqualTo(negativeVector));
                    Assert.That(MaxBounds, Is.EqualTo(negativeVector));
                });
            }
        }

        [TestFixture]
        public class FungeSpaceIOTests : FungeSpaceTests
        {
            private const string TestString = ">2.@";
            private const string TestString2 = ">2v\n@.<";
            private const string TestString3 = ">2v\n  h\f@.<\n  ^";
            private string TestString4;


            [OneTimeSetUp]
            public void Setup()
            {
                TestString4 = new StringBuilder().Insert(0, new string('>', 1000) + "\n", 1000).ToString().TrimEnd();
            }
            
            [Test]
            public void LoadFile_ThrowsException_WhenFileDoesNotExist()
            {
                Assert.That(File.Exists("nonexistent.bf"), Is.False);
                Assert.That(() => _space1D.LoadFile(new FungeVector(), "nonexistent.bf"), Throws.Exception);
            }

            [Test, Description("This test depends on the WriteToString method working correctly. If it fails, check that method first.")]
            public void LoadString_Loads1DString_Into1DSpace()
            {
                _space1D.LoadString(new FungeVector(), TestString);

                FungeVector MinBounds = _space1D.MinBounds;
                FungeVector MaxBounds = _space1D.MaxBounds;
                string contents = _space1D.WriteToString(MinBounds, MaxBounds + new FungeVector(1,1,1) - MinBounds);
                Assert.Multiple(() =>
                {
                    Assert.That(MinBounds, Is.EqualTo(new FungeVector(0)));
                    Assert.That(MaxBounds, Is.EqualTo(new FungeVector(3)));
                    Assert.That(contents, Is.EqualTo(TestString));
                });
            }

            [Test, Description("This test depends on the WriteToString method working correctly. If it fails, check that method first.")]
            public void LoadString_Loads2DString_Into2DSpace()
            {
                _space2D.LoadString(new FungeVector(), TestString2);

                FungeVector MinBounds = _space2D.MinBounds;
                FungeVector MaxBounds = _space2D.MaxBounds;
                string contents = _space2D.WriteToString(MinBounds, MaxBounds + new FungeVector(1, 1) - MinBounds);
                Assert.Multiple(() =>
                {
                    Assert.That(MinBounds, Is.EqualTo(new FungeVector(0, 0)));
                    Assert.That(MaxBounds, Is.EqualTo(new FungeVector(2, 1)));
                    Assert.That(contents, Is.EqualTo(TestString2));
                });
            }

            [Test, Description("This test depends on the WriteToString method working correctly. If it fails, check that method first.")]
            public void LoadString_Loads2DString_Into1DSpace()
            {
                _space1D.LoadString(new FungeVector(), TestString2);
                string expectedContents = ">2v@.<"; // Newline character is ignored

                FungeVector MinBounds = _space1D.MinBounds;
                FungeVector MaxBounds = _space1D.MaxBounds;
                string contents = _space1D.WriteToString(MinBounds, MaxBounds + new FungeVector(1) - MinBounds);
                Assert.Multiple(() =>
                {
                    Assert.That(MinBounds, Is.EqualTo(new FungeVector(0)));
                    Assert.That(MaxBounds, Is.EqualTo(new FungeVector(5)));
                    Assert.That(contents, Is.EqualTo(expectedContents));
                });
            }

            [Test, Description("This test depends on the WriteToString method working correctly. If it fails, check that method first.")]
            public void LoadString_Loads3DString_Into3DSpace()
            {
                _space3D.LoadString(new FungeVector(), TestString3);
                
                FungeVector minBounds = _space3D.MinBounds;
                FungeVector maxBounds = _space3D.MaxBounds;
                string contents = _space3D.WriteToString(minBounds, maxBounds + new FungeVector(1,1,1) - minBounds);
                Assert.Multiple(() =>
                {
                    Assert.That(minBounds, Is.EqualTo(new FungeVector(0, 0, 0)));
                    Assert.That(maxBounds, Is.EqualTo(new FungeVector(2, 1, 1)));
                    Assert.That(contents, Is.EqualTo(TestString3));
                });
            }

            [Test, Description("This test depends on the WriteToString method working correctly. If it fails, check that method first.")]
            public void LoadString_Loads3DString_Into2DSpace()
            {
                _space2D.LoadString(new FungeVector(), TestString3);
                string expectedContents = ">2v   \n  h@.<\n  ^   "; // Form feed character is ignored

                FungeVector MinBounds = _space2D.MinBounds;
                FungeVector MaxBounds = _space2D.MaxBounds;
                string contents = _space2D.WriteToString(MinBounds, MaxBounds + new FungeVector(1,1) - MinBounds);
                Assert.Multiple(() =>
                {
                    Assert.That(MinBounds, Is.EqualTo(new FungeVector(0, 0)));
                    Assert.That(MaxBounds, Is.EqualTo(new FungeVector(5, 2)));
                    Assert.That(contents, Is.EqualTo(expectedContents));
                });
            }

            [Test, Description("This test depends on the WriteToString method working correctly. If it fails, check that method first.")]
            public void LoadString_Loads3DString_Into1DSpace()
            {
                _space1D.LoadString(new FungeVector(), TestString3);
                string expectedContents = ">2v  h@.<  ^"; // Newline and form feed characters are ignored

                FungeVector MinBounds = _space1D.MinBounds;
                FungeVector MaxBounds = _space1D.MaxBounds;
                string contents = _space1D.WriteToString(MinBounds, MaxBounds + new FungeVector(1) - MinBounds);
                Assert.Multiple(() =>
                {
                    Assert.That(MinBounds, Is.EqualTo(new FungeVector(0)));
                    Assert.That(MaxBounds, Is.EqualTo(new FungeVector(11)));
                    Assert.That(contents, Is.EqualTo(expectedContents));
                });
            }

            [Test, Description("This test depends on the WriteToString method working correctly. If it fails, check that method first.")]
            public void LoadString_LoadsVeryLargeString()
            {
                _space2D.LoadString(new FungeVector(), TestString4);
                string expectedContents = new StringBuilder().Insert(0, new string('>', 1000) + "\n", 1000).ToString().TrimEnd();

                FungeVector MinBounds = _space2D.MinBounds;
                FungeVector MaxBounds = _space2D.MaxBounds;
                string contents = _space2D.WriteToString(MinBounds, MaxBounds + new FungeVector(1,1) - MinBounds);
                Assert.Multiple(() =>
                {
                    Assert.That(MinBounds, Is.EqualTo(new FungeVector(0, 0)));
                    Assert.That(MaxBounds, Is.EqualTo(new FungeVector(999, 999)));
                    Assert.That(contents, Is.EqualTo(expectedContents));
                });
            }

            [Test, Description("This test depends on the WriteToString method working correctly. If it fails, check that method first.")]
            public void LoadString_LoadsBinaryString()
            {
                FungeVector position = new();
                Assert.That(position, Is.EqualTo(new FungeVector(0)));
                _space1D.LoadString(position, TestString3, true);

                string expectedContents = ">2v\n  h\f@.<\n  ^";

                FungeVector MinBounds = _space1D.MinBounds;
                FungeVector MaxBounds = _space1D.MaxBounds;
                string contents = _space1D.WriteToString(MinBounds, MaxBounds + new FungeVector(1) - MinBounds);
                Assert.Multiple(() =>
                {
                    Assert.That(MinBounds, Is.EqualTo(position));
                    Assert.That(MaxBounds, Is.EqualTo(new FungeVector(14)));
                    Assert.That(contents, Is.EqualTo(expectedContents));
                });
            }

            [Test]
            public void LoadString_LoadsStringIntoCorrectPosition()
            {
                FungeVector position = new(1, 2, 3);
                _space3D.LoadString(position, TestString3);

                FungeVector MinBounds = _space3D.MinBounds;
                FungeVector MaxBounds = _space3D.MaxBounds;
                Assert.Multiple(() =>
                {
                    Assert.That(MinBounds, Is.EqualTo(position));
                    Assert.That(MaxBounds, Is.EqualTo(new FungeVector(3, 3, 4)));
                });
            }

            [Test]
            public void LoadString_LoadsStringIntoCorrectPosition_WhenNegative()
            {
                FungeVector position = new(-1, -2, -3);
                _space3D.LoadString(position, TestString3);

                FungeVector MinBounds = _space3D.MinBounds;
                FungeVector MaxBounds = _space3D.MaxBounds;
                Assert.Multiple(() =>
                {
                    Assert.That(MinBounds, Is.EqualTo(position));
                    Assert.That(MaxBounds, Is.EqualTo(new FungeVector(1, -1, -2)));
                });
            }

            [Test]
            public void LoadString_ReturnsCorrectSize()
            {
                FungeVector size = _space3D.LoadString(new FungeVector(), TestString3);

                Assert.That(size, Is.EqualTo(new FungeVector(3, 2, 2)));
            }

            [Test]
            public void LoadString_ReturnsCorrectSize_InDifferentAreaOfSpace()
            {
                FungeVector size = _space3D.LoadString(new FungeVector(48, -22, 324), TestString3);

                Assert.That(size, Is.EqualTo(new FungeVector(3, 2, 2)));
            }
            
            [Test]
            public void WriteToString_LinearWritesCorrectly()
            {
                _space3D.LoadString(new FungeVector(), TestString3);
                string expectedContents = ">2v\n  h\f@.<\n  ^";

                string contents = _space3D.WriteToString(new FungeVector(), new FungeVector(3, 3, 3), true);
                Assert.That(contents, Is.EqualTo(expectedContents));
            }
            
            [Test]
            public void WriteToString_ThrowsException_WhenSizeIsBelowZero()
            {
                _space3D.LoadString(new FungeVector(), TestString3);
                Assert.That(() => _space3D.WriteToString(new FungeVector(), new FungeVector(-1, -1, -1)), Throws.ArgumentException);
            }
            
            [Test]
            public void WriteToString_WritesCorrectly_WhenSpaceIsEmpty()
            {
                string contents = _space3D.WriteToString(new FungeVector(-1, -1), new FungeVector(3, 3, 1));
                Assert.That(contents, Is.EqualTo("   \n   \n   "));
            }
        }
    }
}

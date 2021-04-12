using NUnit.Framework;
using VoiceChanger.NoteRecognizer;

namespace VoiceChangerTests.NoteRecognizerTests
{
    public class GuitarTuningNotesCreator_Tests
    {
        private const float Epsilon = 1e-2f;

        [Test]
        public void GetNoteFrequency()
        {
            Assert.AreEqual(440, GuitarTuningNotesCreator.BaseFrequency, Epsilon);

            var n0 = GuitarTuningNotesCreator.GetNoteFrequency(GuitarTuningNotesCreator.BaseFrequency, 0);
            Assert.AreEqual(440.00, n0, Epsilon);
            var n1 = GuitarTuningNotesCreator.GetNoteFrequency(GuitarTuningNotesCreator.BaseFrequency, 1);
            Assert.AreEqual(466.16, n1, Epsilon);
            var n2 = GuitarTuningNotesCreator.GetNoteFrequency(GuitarTuningNotesCreator.BaseFrequency, 2);
            Assert.AreEqual(493.88, n2, Epsilon);
            var n3 = GuitarTuningNotesCreator.GetNoteFrequency(GuitarTuningNotesCreator.BaseFrequency, 3);
            Assert.AreEqual(523.25, n3, Epsilon);
            var n4 = GuitarTuningNotesCreator.GetNoteFrequency(GuitarTuningNotesCreator.BaseFrequency, 4);
            Assert.AreEqual(554.36, n4, Epsilon);
            var n5 = GuitarTuningNotesCreator.GetNoteFrequency(GuitarTuningNotesCreator.BaseFrequency, 5);
            Assert.AreEqual(587.32, n5, Epsilon);
            var n6 = GuitarTuningNotesCreator.GetNoteFrequency(GuitarTuningNotesCreator.BaseFrequency, 6);
            Assert.AreEqual(622.26, n6, Epsilon);
            var n7 = GuitarTuningNotesCreator.GetNoteFrequency(GuitarTuningNotesCreator.BaseFrequency, 7);
            Assert.AreEqual(659.26, n7, Epsilon);
            var n12 = GuitarTuningNotesCreator.GetNoteFrequency(GuitarTuningNotesCreator.BaseFrequency, 12);
            Assert.AreEqual(880, n12, Epsilon);

            var m1 = GuitarTuningNotesCreator.GetNoteFrequency(GuitarTuningNotesCreator.BaseFrequency, -1);
            Assert.AreEqual(415.30, m1, Epsilon);
            var m2 = GuitarTuningNotesCreator.GetNoteFrequency(GuitarTuningNotesCreator.BaseFrequency, -2);
            Assert.AreEqual(392.00, m2, Epsilon);
            var m3 = GuitarTuningNotesCreator.GetNoteFrequency(GuitarTuningNotesCreator.BaseFrequency, -3);
            Assert.AreEqual(369.99, m3, Epsilon);
            var m4 = GuitarTuningNotesCreator.GetNoteFrequency(GuitarTuningNotesCreator.BaseFrequency, -4);
            Assert.AreEqual(349.23, m4, Epsilon);
            var m5 = GuitarTuningNotesCreator.GetNoteFrequency(GuitarTuningNotesCreator.BaseFrequency, -5);
            Assert.AreEqual(329.63, m5, Epsilon);
            var m6 = GuitarTuningNotesCreator.GetNoteFrequency(GuitarTuningNotesCreator.BaseFrequency, -6);
            Assert.AreEqual(311.13, m6, Epsilon);
            var m7 = GuitarTuningNotesCreator.GetNoteFrequency(GuitarTuningNotesCreator.BaseFrequency, -7);
            Assert.AreEqual(293.66, m7, Epsilon);
            var m12 = GuitarTuningNotesCreator.GetNoteFrequency(GuitarTuningNotesCreator.BaseFrequency, -12);
            Assert.AreEqual(220, m12, Epsilon);
        }

        [Test]
        public void GetStringFrequency()
        {
            var s1t0 = GuitarTuningNotesCreator.GetStringFrequency(1, 0);
            Assert.AreEqual(329.63, s1t0, Epsilon);
            var s1t1 = GuitarTuningNotesCreator.GetStringFrequency(1, 1);
            Assert.AreEqual(349.23, s1t1, Epsilon);
            var s1t2 = GuitarTuningNotesCreator.GetStringFrequency(1, 2);
            Assert.AreEqual(369.99, s1t2, Epsilon);
            var s1t3 = GuitarTuningNotesCreator.GetStringFrequency(1, 3);
            Assert.AreEqual(391.99, s1t3, Epsilon);
            var s1t4 = GuitarTuningNotesCreator.GetStringFrequency(1, 4);
            Assert.AreEqual(415.30, s1t4, Epsilon);
            var s1t5 = GuitarTuningNotesCreator.GetStringFrequency(1, 5);
            Assert.AreEqual(440.00, s1t5, Epsilon);

            var s2t0 = GuitarTuningNotesCreator.GetStringFrequency(2, 0);
            Assert.AreEqual(246.94, s2t0, Epsilon);
            var s2t1 = GuitarTuningNotesCreator.GetStringFrequency(2, 1);
            Assert.AreEqual(261.63, s2t1, Epsilon);
            var s2t2 = GuitarTuningNotesCreator.GetStringFrequency(2, 2);
            Assert.AreEqual(277.18, s2t2, Epsilon);
            var s2t3 = GuitarTuningNotesCreator.GetStringFrequency(2, 3);
            Assert.AreEqual(293.66, s2t3, Epsilon);
            var s2t4 = GuitarTuningNotesCreator.GetStringFrequency(2, 4);
            Assert.AreEqual(311.13, s2t4, Epsilon);
            var s2t5 = GuitarTuningNotesCreator.GetStringFrequency(2, 5);
            Assert.AreEqual(329.63, s2t5, Epsilon);

            var s3t0 = GuitarTuningNotesCreator.GetStringFrequency(3, 0);
            Assert.AreEqual(196.00, s3t0, Epsilon);
            var s3t1 = GuitarTuningNotesCreator.GetStringFrequency(3, 1);
            Assert.AreEqual(207.65, s3t1, Epsilon);
            var s3t2 = GuitarTuningNotesCreator.GetStringFrequency(3, 2);
            Assert.AreEqual(220.00, s3t2, Epsilon);
            var s3t3 = GuitarTuningNotesCreator.GetStringFrequency(3, 3);
            Assert.AreEqual(233.08, s3t3, Epsilon);
            var s3t4 = GuitarTuningNotesCreator.GetStringFrequency(3, 4);
            Assert.AreEqual(246.94, s3t4, Epsilon);
            var s3t5 = GuitarTuningNotesCreator.GetStringFrequency(3, 5);
            Assert.AreEqual(261.63, s3t5, Epsilon);

            var s4t0 = GuitarTuningNotesCreator.GetStringFrequency(4, 0);
            Assert.AreEqual(146.83, s4t0, Epsilon);
            var s4t1 = GuitarTuningNotesCreator.GetStringFrequency(4, 1);
            Assert.AreEqual(155.56, s4t1, Epsilon);
            var s4t2 = GuitarTuningNotesCreator.GetStringFrequency(4, 2);
            Assert.AreEqual(164.81, s4t2, Epsilon);
            var s4t3 = GuitarTuningNotesCreator.GetStringFrequency(4, 3);
            Assert.AreEqual(174.61, s4t3, Epsilon);
            var s4t4 = GuitarTuningNotesCreator.GetStringFrequency(4, 4);
            Assert.AreEqual(185.00, s4t4, Epsilon);
            var s4t5 = GuitarTuningNotesCreator.GetStringFrequency(4, 5);
            Assert.AreEqual(196.00, s4t5, Epsilon);

            var s5t0 = GuitarTuningNotesCreator.GetStringFrequency(5, 0);
            Assert.AreEqual(110.00, s5t0, Epsilon);
            var s5t1 = GuitarTuningNotesCreator.GetStringFrequency(5, 1);
            Assert.AreEqual(116.54, s5t1, Epsilon);
            var s5t2 = GuitarTuningNotesCreator.GetStringFrequency(5, 2);
            Assert.AreEqual(123.47, s5t2, Epsilon);
            var s5t3 = GuitarTuningNotesCreator.GetStringFrequency(5, 3);
            Assert.AreEqual(130.81, s5t3, Epsilon);
            var s5t4 = GuitarTuningNotesCreator.GetStringFrequency(5, 4);
            Assert.AreEqual(138.59, s5t4, Epsilon);
            var s5t5 = GuitarTuningNotesCreator.GetStringFrequency(5, 5);
            Assert.AreEqual(146.83, s5t5, Epsilon);

            var s6t0 = GuitarTuningNotesCreator.GetStringFrequency(6, 0);
            Assert.AreEqual(82.41, s6t0, Epsilon);
            var s6t1 = GuitarTuningNotesCreator.GetStringFrequency(6, 1);
            Assert.AreEqual(87.31, s6t1, Epsilon);
            var s6t2 = GuitarTuningNotesCreator.GetStringFrequency(6, 2);
            Assert.AreEqual(92.50, s6t2, Epsilon);
            var s6t3 = GuitarTuningNotesCreator.GetStringFrequency(6, 3);
            Assert.AreEqual(98.00, s6t3, Epsilon);
            var s6t4 = GuitarTuningNotesCreator.GetStringFrequency(6, 4);
            Assert.AreEqual(103.83, s6t4, Epsilon);
            var s6t5 = GuitarTuningNotesCreator.GetStringFrequency(6, 5);
            Assert.AreEqual(110.00, s6t5, Epsilon);
        }

        [Test]
        public void GetStringFrequenciesRange()
        {
            var range = GuitarTuningNotesCreator.GetStringFrequenciesRange(1, 5);
            Assert.AreEqual(5, range.Count);
            for (int i = 0; i < 5; i++)
            {
                var frequency = GuitarTuningNotesCreator.GetStringFrequency(1, i);
                Assert.AreEqual(frequency, range[i]);
            }
        }

        [Test]
        public void GetStringsFrequencies()
        {
            var frequencies = GuitarTuningNotesCreator.GetStringsFrequencies(6);
            Assert.AreEqual(30, frequencies.Count);
        }
    }
}

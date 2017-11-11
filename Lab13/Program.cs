using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Lab13 {
    class Program {
        struct Complex {
            public double re, im;

            public Complex(double r, double i = 0) {
                re = r;
                im = i;
            }

            public static Complex operator +(Complex x, Complex y) {
                return new Complex(x.re + y.re, x.im + y.im);
            }

            public static Complex operator -(Complex x, Complex y) {
                return new Complex(x.re - y.re, x.im - y.im);
            }

            public static Complex operator *(Complex x, Complex y) {
                return new Complex(x.re * y.re - x.im * y.im, x.re * y.im + y.re * x.im);
            }

            public static Complex operator /(Complex x, int n) {
                return new Complex(x.re / n, x.im / n);
            }

            public static Complex[] get(string s, int n) {
                Complex[] res = new Complex[n];

                for (int i = 0; i < s.Length; i++)
                    res[i] = new Complex(i < s.Length ? s[s.Length - 1 - i] - '0' : 0);

                return res;
            }

            public static Complex[] getPows(double re, double im, int n) {
                Complex[] w = new Complex[n];
                Complex pow = new Complex(1);
                Complex x = new Complex(re, im);

                for (int i = 0; i < n; i++) {
                    w[i] = new Complex(pow.re, pow.im);
                    pow *= x;
                }

                return w;
            }
        }

        // fast fourier transform
        static void fft(ref Complex[] a, bool invert) {
            int n = a.Length;

            for (int i = 1, j = 0; i < n; ++i) {
                int bit = n >> 1;

                while (j >= bit) {
                    j -= bit;
                    bit >>= 1;
                }

                j += bit;

                if (i < j) {
                    Complex tmp = a[i];
                    a[i] = a[j];
                    a[j] = tmp;
                }
            }

            for (int len = 2; len <= n; len <<= 1) {
                double ang = 2 * Math.PI / len * (invert ? -1 : 1);

                Complex[] pows = Complex.getPows(Math.Cos(ang), Math.Sin(ang), len / 2);

                for (int i = 0; i < n; i += len) {
                    for (int j = 0; j < len / 2; j++) {
                        Complex u = a[i + j];
                        Complex v = a[i + j + len / 2] * pows[j];

                        a[i + j] += v;
                        a[i + j + len / 2] = u - v;
                    }
                } 
            }

            if (invert)
                for (int i = 0; i < n; i++)
                    a[i] /= n;
        }

        static string multiply(string a, string b) {
            int n = 2;
            int max = 2 * Math.Max(a.Length, b.Length);

            while (n < max)
                n *= 2;

            Complex[] fa = Complex.get(a, n);
            Complex[] fb = Complex.get(b, n);

            fft(ref fa, false);
            fft(ref fb, false);

            for (int i = 0; i < n; i++)
                fa[i] *= fb[i];

            fft(ref fa, true);

            int v;
            string mult = "";
            int carry = 0;

            for (int i = 0; i < n; i++) {
                v = (int)(fa[i].re + 0.5) + carry;
                carry = v / 10;
                mult = (v % 10).ToString() + mult;
            }

            return mult == "" ? 0.ToString() : mult;
        }

        static long sum(string n) {
            long sum = 0;
            for (int i = 0; i < n.Length; i++)
                sum += n[i] - '0';

            return sum;
        }

        static void Main(string[] args) {
            StreamReader fs = new StreamReader("input_3_2.txt");
            StreamWriter fres = new StreamWriter("output_3_2.txt");

            int n = int.Parse(fs.ReadLine());

            for (int i = 0; i < n; i++) { 
                string[] s = fs.ReadLine().Split(' ');

                fres.WriteLine(sum(multiply(s[0], s[1])));
            }

            fs.Close();
            fres.Close();
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HashCode
{
    class Program
    {
        static void Main(string[] args)
        {

            string[] files = { "a_example", "b_lovely_landscapes", "c_memorable_moments", "d_pet_pictures", "e_shiny_selfies" };
            foreach (string file in files)
            {
                Console.WriteLine(file);
                // try
                {
                    StreamReader sr = new StreamReader("input/" + file + ".txt");
                    int numPhotos = int.Parse(sr.ReadLine());
                    List<Photo> photos = new List<Photo>(numPhotos);
                    for (int i = 0; i < numPhotos; i++)
                    {
                        List<string> line = new List<string>(sr.ReadLine().Split(' '));
                        bool horizontal = line[0].Equals("H");
                        line.RemoveRange(0, 2);
                        photos.Add(new Photo(i, line, horizontal));
                    }
                    SlideShowSolver sol = new SlideShowSolver(photos);
                    sol.Solve(file + ".txt");
                    sr.Close();
                }
                // catch
                // {
                //     throw;
                // }
            }
        }
    }

    class Photo : IComparable
    {
        public int Id;
        public List<string> Tags;
        public bool Horizontal;

        public Photo(int id, List<string> tags, bool horizontal)
        {
            this.Id = id;
            this.Tags = tags;
            this.Horizontal = horizontal;
        }
        private static int CountCommonTags(Photo a, Photo b)
        {
            List<string> tags = new List<string>();
            int count = 0;
            foreach (string tag in a.Tags)
            {
                if (!tags.Contains(tag))
                {
                    count++;
                    tags.Add(tag);
                }
            }
            foreach (string tag in b.Tags)
            {
                if (!tags.Contains(tag))
                {
                    count++;
                    tags.Add(tag);
                }
            }
            return count;
        }
        int IComparable.CompareTo(object obj)
        {
            // Photo other = (Photo)obj;
            // int comp = other.Tags.Count.CompareTo(this.Tags.Count);
            // if (comp == 0)
            // {
            //     if (this.Horizontal && !other.Horizontal)
            //     {
            //         comp = -1;
            //     }
            //     else if (other.Horizontal && !this.Horizontal)
            //     {
            //         comp = 1;
            //     }
            // }
            // return comp;
            Photo other = (Photo)obj;
            int commonTags = CountCommonTags(this, other);
            float thisRatio = commonTags / this.Tags.Count;
            float otherRatio = commonTags / other.Tags.Count;
            int comp = thisRatio.CompareTo(otherRatio);
            if (comp == 0)
            {
                if (this.Horizontal && !other.Horizontal)
                {
                    comp = 1;
                }
                else if (other.Horizontal && !this.Horizontal)
                {
                    comp = -1;
                }
            }
            return comp;
        }
    }

    class Slide
    {
        public List<Photo> Photos;
        public List<string> Tags;
        public Slide(Photo photo)
        {
            Photos = new List<Photo>(1);
            Photos.Add(photo);
            this.Tags = photo.Tags;
        }
        public Slide(Photo photo1, Photo photo2)
        {
            Photos = new List<Photo>(2);
            Photos.Add(photo1);
            Photos.Add(photo2);
            this.Tags = new List<string>();
            foreach (Photo photo in this.Photos)
            {
                foreach (string tag in photo.Tags)
                {
                    if (!this.Tags.Contains(tag))
                    {
                        this.Tags.Add(tag);
                    }
                }
            }
        }

        private static int CountCommonTags(Slide a, Slide b)
        {
            List<string> tags = new List<string>();
            int count = 0;
            foreach (string tag in a.Tags)
            {
                if (!tags.Contains(tag))
                {
                    count++;
                    tags.Add(tag);
                }
            }
            foreach (string tag in b.Tags)
            {
                if (!tags.Contains(tag))
                {
                    count++;
                    tags.Add(tag);
                }
            }
            return count;
        }
        public static int ScoreTransition(Slide a, Slide b)
        {
            int tagsInCommon = Slide.CountCommonTags(a, b);
            int ab = a.Tags.Count - tagsInCommon;
            int ba = b.Tags.Count - tagsInCommon;
            return Math.Min(tagsInCommon, Math.Min(ab, ba));
        }

    }

    class SlideShowSolver
    {
        public List<Photo> Photos;
        public SlideShowSolver(List<Photo> photos)
        {
            this.Photos = photos;
        }

        public void Solve(string outputName)
        {
            List<Slide> slideShow = new List<Slide>();
            Photos.Sort(); // Sort by number of tags, H before V
            Photo lastV = null;
            for (int i = 0; i < Photos.Count; i++) // go down list and add every photo, combining vertical slides into a single one
            {
                Photo photo = Photos[i];
                Slide newSlide = new Slide(photo);
                if (!photo.Horizontal)
                {
                    if (lastV == null)
                    {
                        lastV = photo;
                        continue;
                    }
                    else
                    {
                        newSlide = new Slide(photo, lastV);
                        lastV = null;
                    }
                }

                slideShow.Add(newSlide);
            }

            for (int i = 0; i < slideShow.Count - 1; i++)
            {
                if (Slide.ScoreTransition(slideShow[i], slideShow[i + 1]) == 0)
                {
                    if (i + 2 < slideShow.Count)
                    {
                        Slide temp = slideShow[i + 2];
                        slideShow[i + 2] = slideShow[i + 1];
                        slideShow[i + 1] = temp;
                    }
                }
            }

            StreamWriter sw = new StreamWriter("output/" + outputName);
            sw.WriteLine(slideShow.Count);
            foreach (Slide slide in slideShow)
            {
                if (slide.Photos.Count == 2)
                {
                    sw.WriteLine(slide.Photos[0].Id + " " + slide.Photos[1].Id);
                }
                else
                {
                    sw.WriteLine(slide.Photos[0].Id);
                }
            }
            sw.Close();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeakDanish.Forms.Services
{
    public interface INavigationService
    {
        Task<bool> GoBackAsync(bool isAnimated = true);
        Task<bool> NavigateAsync(string path, bool isAnimated = true);
    }

    public class NavigationService : INavigationService
    {
        public async Task<bool> GoBackAsync(bool isAnimated = true)
        {
            try
            {
                await Shell.Current.GoToAsync("..", isAnimated);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> NavigateAsync(string path, bool isAnimated = true)
        {
            try
            {
                await Shell.Current.GoToAsync(path, isAnimated);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            
        }
    }
}

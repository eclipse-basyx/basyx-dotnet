using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using BaSyx.Models.AdminShell;

namespace BaSyx.Utils.IdShortPathResolver
{
    /// <summary>
    /// Class to get a hierarchical SubmodelElement in a Submodel by its IdShort.
    /// </summary>
    public class IdShortPathResolver
    {
        public const char PATH_SEPERATOR = '.';
        private readonly IElementContainer<ISubmodelElement> _submodelElements;

        public IdShortPathResolver(IElementContainer<ISubmodelElement> submodelElements)
        {
            _submodelElements = submodelElements;
        }

        /// <summary>
        /// Returns the nestes SubmodelElement given in the idShortPath.
        /// </summary>
        /// <param name="idShortPath">The idShortPath of the SubmodelElement</param>
        /// <returns>the nested SubmodelElement</returns>
        public IElementContainer<ISubmodelElement> GetChild(string idShortPath)
        {
            Stack<string> idShortStack = SplitIdShortPaths(idShortPath);
            return GetLastElementOfStack(idShortStack);
        }

        /// <summary>
        /// Splits an idShortPath into its single parts.
        /// </summary>
        /// <param name="fullIdShortPath"></param>
        /// <returns></returns>
        private Stack<string> SplitIdShortPaths(string fullIdShortPath)
        {
            try
            {
                string[] splittedPath =
                    fullIdShortPath.Split(new char[] { PATH_SEPERATOR }, StringSplitOptions.RemoveEmptyEntries);
                return GenerateStackFromSplittedPath(splittedPath);
            }
            catch (InvalidDataException e)
            {
                Console.WriteLine($"Element with index {fullIdShortPath} does not exist.");
                throw;
            }
        }

        /// <summary>
        /// Generates a stack from the splitted path by looping the single parts backwards.
        /// </summary>
        /// <param name="splittedPath"></param>
        /// <returns></returns>
        private Stack<string> GenerateStackFromSplittedPath(string[] splittedPath)
        {
            var stack = new Stack<string>();
            for(int i = splittedPath.Length - 1; i >= 0; i--)
            {
                List<int> indices = GetAllIndices(splittedPath[i]);
                for (int ix = indices.Count - 1; ix >= 0; ix--)
                {
                    stack.Push(indices[ix].ToString());
                }
                stack.Push(splittedPath[i].Split('[').First());
            }

            return stack;
        }

        private List<int> GetAllIndices(string idShort)
        {
            var indices = new List<int>();
            while (GetIndexOfOpeningBracket(idShort) != -1)
            {
                int occurrence = GetIndexOfOpeningBracket(idShort);
                int end = GetIndexOfClosingBracket(idShort);

                ValidateIndexSyntax(occurrence, end, idShort);
                int index = int.Parse(idShort.Substring(occurrence + 1, end - occurrence - 1));

                // Check if index is invalid
                if (index < 0)
                    throw new InvalidDataException();

                indices.Add(index);
                idShort = idShort.Substring(end + 1);
            }
            // Check if too many closing brackets exist
            if (GetIndexOfClosingBracket(idShort) != -1)
                throw new InvalidDataException();

            return indices;
        }

        private int GetIndexOfOpeningBracket(string idShort)
        {
            return idShort.IndexOf('[');
        }

        private int GetIndexOfClosingBracket(string idShort)
        {
            return idShort.IndexOf(']');
        }

        private void ValidateIndexSyntax(int occurrence, int end, string idShort)
        {
            // Closing bracket is missing
            if (end == -1)
                throw new InvalidDataException();

            // Invalid character after closing bracket
            if (idShort.Count() - 1 > end)
            {
                if (!idShort.Substring(end + 1, 1).Equals("["))
                    throw new InvalidDataException();
            }

            // Opening bracket inside brackets
            if (idShort.Substring(occurrence + 1, end - occurrence - 1).Contains("["))
                throw new InvalidDataException();
        }

        private IElementContainer<ISubmodelElement> GetLastElementOfStack(Stack<string> idShortStack)
        {
            var idShort = idShortStack.Pop();
            if (!_submodelElements.HasChild(idShort))
                return null;
            IElementContainer<ISubmodelElement> element = _submodelElements.GetChild(idShort);
            
            while (idShortStack.Count != 0)
            {
                idShort = idShortStack.Pop();
                element = GetOrSerializeElement(element);
                if(!element.HasChild(idShort))
                {
                    element = null;
                    break;
                }
                element = element.GetChild(idShort);
            }

            return element;
        }

        private IElementContainer<ISubmodelElement> GetOrSerializeElement(IElementContainer<ISubmodelElement> element)
        {
            if (!element.HasChildren() && element is SubmodelElementCollection smc)
            {
                var value = smc.Get?.Invoke(smc).Result;
                element = (IElementContainer<ISubmodelElement>)value.Value;
            }
            else if (!element.HasChildren() && element is SubmodelElementList sml)
            {
                var value = sml.Get?.Invoke(sml).Result;
                element = (IElementContainer<ISubmodelElement>)value.Value;
            }
            else if (!element.HasChildren() && element is Entity ent)
            {
                var value = ent.Get?.Invoke(ent).Result;
                element = (IElementContainer<ISubmodelElement>)value.Statements;
            }

            return element;
        }
    }
}

with open('README.md', 'r') as f:
    text = f.read()

text += """
## Custom Implementations & Nested Projects
- **Nested Projects**: `layam-academy` and `sangam-credit` have been fully equipped with their `dogfood.sh` invoking CanonFlow, generating FsCheck property tests, and asserting TypeScript Zod logic using Jest.
- **Custom Implementations**: 
  - `migration-demo`: Included dummy test infrastructure and verified that CanonFlow correctly executes migration diagnostics on V1->V2.
  - `arangetram-adversaries`: Safely demonstrated that impossible adversarial schemas halt gracefully without emitting broken tests, fulfilling its objective.
"""

with open('README.md', 'w') as f:
    f.write(text)

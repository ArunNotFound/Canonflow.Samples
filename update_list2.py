import re

with open('list.md', 'r') as f:
    text = f.read()

for sample in ['Migration Engine Demo']:
    text = re.sub(
        rf"(### \d+\. {sample} \([^)]+\)\n)- \[ \]",
        r"\1- [x]",
        text
    )
    text = re.sub(
        rf"(### \d+\. {sample} \([^)]+\)\n- \[x\].*\n)- \[ \](.*)\n- \[ \](.*)\n- \[ \](.*)\n",
        r"\1- [x]\2\n- [x]\3\n- [x]\4\n",
        text
    )

with open('list.md', 'w') as f:
    f.write(text)

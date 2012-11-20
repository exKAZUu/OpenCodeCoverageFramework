# How to manage issues
1. Create issues on GitHub
2. Create new branch with the following command (XX is the issue number and YYYY is the abstract):

        git pull
        git checkout -b XX_YYYY
    Note that this command create a branch from the current branch.
3. Write and commit code!
4. Commit the final code with the following command

        git commit -m "brah brah brah ... Close #XX"
5. Merge the branch into the "master" branch

        git checkout master
        git merge XX_YYYY
        git push origin master
        git branch -d XX_YYYY
    Note that git shows errors if the merge is failed.

# Development team
Kazunori Sakamoto

using System.Collections.Generic;
using System.Diagnostics;

namespace Gitcheatsheet.Model
{
    [DebuggerDisplay("{Leader}")]
    public class CheatSheet
    {
        public string Leader { get; private set; }
        public List<Section> Sections { get; private set; }

        Section AddSection(string title)
        {
            var sec = new Section(title);
            Sections.Add(sec);
            return sec;
        }

        public CheatSheet()
        {
            Leader = "Each command shown below should be prefixed with \"git\", for example\n\"git init\".";
            Sections = new List<Section>();

            // Max line length = 120 characters.
            AddGettingStarted();
            AddDailyWork();
            AddBranching();
            AddRemotes();
            AddReviewing();
            AddMisc1();
        }

        void AddGettingStarted()
        {
            var sec = AddSection("Getting Started");
            var subsec = sec.AddSubSection("Do this first!!!");

            subsec.AddCheat(",NG", "Setup .gitattributes correctly",
                "Every project should have a .gitattributes file that says how to deal with line endings, it will prevent " +
                "huge amounts of grief caused by people not setting up their configuration correctly. By using a " +
                ".gitattributes file that choice is made in the repository, not left up to each individual user. The " +
                "example file linked here should be good enough in 99% of cases.",
                "examplegitattributes.txt");
            subsec.AddCheat(",NG", "Setup .gitignore correctly",
                "Configure your .gitignore for each repository. Example configs are available at " +
                "https://github.com/github/gitignore but beware, the Visual Studio one is very aggressive, it " +
                "will ignore 'bin' folders within node_modules, for example. See my example at the (more) link, " +
                "and also http://git-scm.com/docs/gitignore",
                "examplegitignore.txt");
            subsec.AddCheat(",NG", "Setup ~/.gitconfig correctly",
                "While you can use individual 'git config' commands to set an examine your configuration, it is " +
                "just as easy to edit the file in a text editor. As an absolute minimum, you should set user.name " +
                "and user.email",
                "examplegitconfig.txt");
            subsec.AddCheat(null, "config [--global] --unset user.email",
                "Unset a setting.",
                "http://git-scm.com/docs/git-config");
            subsec.AddCheat(null, "config [--global] --edit",
                "Open the .gitconfig file in your chosen editor.",
                "http://git-scm.com/docs/git-config");
            subsec.AddCheat(null, "config [--global] user.email Phil@foo.com",
                "Set email address to appear in commits.",
                "http://git-scm.com/docs/git-config");
            subsec.AddCheat(null, "config --list",
                "Display current settings",
                "http://git-scm.com/docs/git-config");

            subsec = sec.AddSubSection("Help");
            subsec.AddCheat(null, "verb --help", "Display man page for verb, e.g. 'git commit --help'.");
            subsec.AddCheat(",NG", "man git-verb", "Display man page for verb.");


            subsec = sec.AddSubSection("Repository Creation and Cloning");
            subsec.AddCheat("L", "init",
                "Create new repo (.git folder) in current directory. Does not add any files to start with, you " +
                "need to use 'git add'.",
                "http://git-scm.com/docs/git-init");
            subsec.AddCheat("RLWA", "clone url [mydir]",
                "Create local copy (in the mydir folder) of repo at url. Automatically sets up a remote called " +
                "'origin' that links back to the source and creates local branches to track remote branches and " +
                "checks out the active branch, populating your local workspace.",
                "http://git-scm.com/docs/git-clone");


            subsec = sec.AddSubSection("Using a Bare Repo as a Central Repo");
            subsec.AddCheat("L", "1a. init --bare newrepo.git",
                "Initialise an empty bare repo in the current directory. The normal practice is to do this on a " +
                "central server to which you have access, then clone it or connect an existing repo to it (one of " +
                "2a, 2b or 2c). You will probably have to widen permissions (chmod ugo+rw) on the files in the " +
                ".git folder to enable other users to push to it. It is a convention that bare repos are named with " +
                "a .git suffix.",
                "http://git-scm.com/docs/git-init");
            subsec.AddCheat("L", "1b. clone --bare url",
                 "As an alternative, you can clone an existing repo but do it in a bare form, which doesn't " +
                 "populate the workspace.",
                "http://git-scm.com/docs/git-clone");
            subsec.AddCheat("L", "2a. remote add origin user@server:path/to/newrepo.git",
                "In an existing local repository, add a new SSH remote called origin which connects to your bare " +
                "repo. The path is relative to the home directory of 'user'. The user is often 'git', which can be " +
                "confusing: this is where 'git@github.com' comes from. The syntax for HTTPS is " +
                "https://server/user/project",
                "http://git-scm.com/docs/git-remote");
            subsec.AddCheat("L", "2b. remote set-url origin user@server:path/to/newrepo.git", "Change the remote " +
                "url of an existing remote called 'origin' so that it points to your new bare repo.",
                "http://git-scm.com/docs/git-remote");
            subsec.AddCheat("RLWA", "2c. clone user@server:path/to/newrepo.git",
                "Alternatively, just clone the bare repo (it will have no files) and start adding files locally, " +
                "then just push when you are ready.",
                "http://git-scm.com/docs/git-clone");
            subsec.AddCheat("LRA", "3. push --all origin", "Push all changes to remote called origin.",
                "http://git-scm.com/docs/git-push");


            subsec = sec.AddSubSection("Graphical Tools");
            subsec.AddCheat(null, "gui",
                "Start a graphical tool for making commits, branches etc. See also gitk, a graphical tool for " +
                "browsing history.",
                "http://git-scm.com/docs/git-gui");
            subsec.AddCheat("L,NG", "gitk [--all] [file]",
                "Display graphical repository browser. Accepts the same options as 'git log'. The --all option makes " +
                "it display the branching history for every branch, otherwise it just shows your current branch. " +
                "Pass a file to look at the history of an individual file.",
                "http://git-scm.com/docs/gitk");
            subsec.AddCheat("L", "gui blame filespec", "Graphical blame.",
                "http://git-scm.com/docs/git-gui");
        }

        void AddDailyWork()
        {
            var sec = AddSection("Daily Work");

            var subsec = sec.AddSubSection("Index Management (Staging)");
            subsec.AddCheat("WIA", "add pathspec",
                "Update the index using the current content of the workspace. Pathspec can be a file or directory " +
                "(such as '.'), which will be added recursively. Adding a file to the index is also known as " +
                "'staging' the file.",
                "http://git-scm.com/docs/git-add");
            subsec.AddCheat("WIA", "add -A pathspec",
                "A synonym for '--all'. If you have removed files from your working tree you need to issue this " +
                "command to have the removal added to the index. This will be the default in git v2.0.",
                "http://git-scm.com/docs/git-add");
            subsec.AddCheat("WIA", "add -i pathspec",
                "Add interactively. Can be useful for picking subsets of files.",
                "http://git-scm.com/docs/git-add");
            subsec.AddCheat("WIA", "rm pathspec",
                "Remove file(s) from the workspace AND the index. This command can be used after a /bin/rm, i.e. " +
                "when the file is already physically gone from your workspace. After this command, git is no " +
                "longer tracking the file.",
                "http://git-scm.com/docs/git-rm");
            subsec.AddCheat("I", "rm --cached pathspec",
                "Remove file(s) from the index. The workspace is not touched at all. After this command git is no " +
                "longer tracking the file.",
                "http://git-scm.com/docs/git-rm");
            subsec.AddCheat("WIA", "rm 'foo/*.log'",
                "Glob patterns work. Escape '*' to stop the shell expanding it. See the example at the link for a " +
                "discussion on the effects of quoting.",
                "http://git-scm.com/docs/git-rm");
            subsec.AddCheat("WIA", "mv oldfile newfile",
                "Rename oldfile to newfile. Often unnecessary because git will detect renamed files automatically " +
                "if their content has not greatly changed.",
                "http://git-scm.com/docs/git-mv");


            subsec = sec.AddSubSection("Committing");
            subsec.AddCheat("ILA", "commit [-m \"message\"]",
                "Take the current contents of the index and commit it to the local repo. -m is used to specify a " +
                "commit message. If omitted, the editor will open to allow you to enter the message. (A blank " +
                "message aborts the commit). It is recommended that a commit message consist of 1 line (known as " +
                "the title), then a blank line, then a longer description. See the discussion at the link.",
                "http://git-scm.com/docs/git-commit");
            subsec.AddCheat("WILA", "commit -a",
                "Add all currently tracked files that are changed or deleted to the index and commit. " +
                "Untracked files are NOT added.",
                "http://git-scm.com/docs/git-commit");
            subsec.AddCheat("ILA", "commit --amend [-m \"message\"]",
                "Revise last commit, for example if you missed some files or made a minor typo, specifying a new " +
                "message. WARNING: If you have pushed the commit you are effectively rewriting history and " +
                "subsequent pushes will fail with 'tip of your current branch is behind remote'. Use 'push -f' " +
                "to force your changes up, but don't do this if you are in a team.",
                "http://git-scm.com/docs/git-commit");
            subsec.AddCheat("ILA", "commit --amend --no-edit",
                "Revise last commit, reusing the same message. See warning in previous cheat.",
                "http://git-scm.com/docs/git-commit");
            subsec.AddCheat("WLA", "commit pathspecs",
                "You can give git-commit a list of files; in this case a new commit is made that contains ONLY the " +
                "mentioned files; the index is bypassed and is not modified at all. This is rarely used, but is a " +
                "way to 'jump the queue' without requiring a lot of index manipulation.",
                "http://git-scm.com/docs/git-commit");


            subsec = sec.AddSubSection("Undoing");
            subsec.AddCheat("LIA", "reset [sha] pathspec",
                "Copy files from the repo to the index. If you allow sha to default to HEAD this action " +
                "effectively removes them from the index because they will then match the repo (git status " +
                "mentions this as a way of unstaging a file). However the command is actually more generic than " +
                "that, since sha can be anything, but if you are doing that you probably actually need the next " +
                "command, 'git checkout [sha]'.",
                "http://git-scm.com/docs/git-reset");
            subsec.AddCheat("LWA", "checkout [sha] -- pathspec",
                "Copy files from the repo to the workspace. (See hint in 'git status'). This is how to discard " +
                "your workspace changes. sha defaults to HEAD, but you can specify an older commit: this is how " +
                "to revert a file to a previous state.",
                "http://git-scm.com/docs/git-reset");
            subsec.AddCheat("LWA", "show HEAD~4:File > OldFile",
                "An example of how to recover an old version of a file into a new filename. If you just want to " +
                "replace in your workspace, you can do a 'git checkout' instead.",
                "http://git-scm.com/docs/gitrevisions.html");
            subsec.AddCheat("L", "reset --soft sha",
                "Set HEAD in the repo to sha. Do not modify the index or workspace. The result is that your " +
                "staged and workspace changes are still available.",
                "http://git-scm.com/docs/git-reset");
            subsec.AddCheat("L", "reset --soft HEAD^",
                "Example of the previous: undo the last commit but leave all your changes in the workspace and " +
                "the index.",
                "http://git-scm.com/docs/git-reset");
            subsec.AddCheat("LIA", "reset [--mixed] sha",
                "Set HEAD in the repo to sha. Reset the index but leave the workspace alone. The result is that " +
                "all your workspace WIP is still available, but needs staging. --mixed is the default.",
                "http://git-scm.com/docs/git-reset");
            subsec.AddCheat("LIWA", "reset --hard sha",
                "Set HEAD in the repo to sha. Reset the index and the workspace (WARNING: any changes to " +
                "tracked files in the workspace since sha will be lost). Untracked files however, will be left " +
                "alone.",
                "http://git-scm.com/docs/git-reset");
            subsec.AddCheat("LIWA", "reset --hard HEAD^",
                "Example of the previous: completely blow away the last commit.",
                "http://git-scm.com/docs/git-reset");
            subsec.AddCheat("LIWA", "reset --hard HEAD",
                "Example: abort a merge in progress.",
                "http://git-scm.com/docs/git-reset");
            subsec.AddCheat("LIWA", "reset --hard ORIG_HEAD",
                "Example: undo the last successful merge - after it has been committed! - AND all changes made " +
                "since. Relies on the ORIG_HEAD pointer still being around.",
                "http://git-scm.com/docs/git-reset");


            subsec = sec.AddSubSection("Reverting");
            subsec.AddCheat("LIWA", "revert [shas]",
                "Revert the changes that the shas introduced and record new commits to document this. Your " +
                "workspace must be clean before beginning. n.b. git reset may be what you want, rather than revert " +
                "(see Undoing, above). On the other hand, revert does provide a way to eliminate one or more " +
                "commits from the MIDDLE of the history. A revert is the logical inverse of a cherry-pick.",
                "http://git-scm.com/docs/git-revert");
            subsec.AddCheat("LIWA", "revert --continue",
                "Continue after resolving conflicts.",
                "http://git-scm.com/docs/git-revert");
            subsec.AddCheat("LIWA", "revert --quit",
                "Forget about the current operation in progress.",
                "http://git-scm.com/docs/git-revert");
            subsec.AddCheat("LIWA", "revert --abort",
                "Forget about the current operation in progress and return to the pre-sequence state.",
                "http://git-scm.com/docs/git-revert");


            subsec = sec.AddSubSection("Cleaning");
            subsec.AddCheat("W", "clean [-f] [-d]",
                "Remove untracked files (also directories if -d) from the workspace. -f (force) is normally " +
                "required to get it to run.",
                "http://git-scm.com/docs/git-clean");
            subsec.AddCheat("W", "clean -i",
                "Clean interactively. Does not require -f.",
                "http://git-scm.com/docs/git-clean");
            subsec.AddCheat("W", "clean -n",
                "Dry run. Does not require -f.",
                "http://git-scm.com/docs/git-clean");
            subsec.AddCheat("W", "clean -x",
                "Also remove ignored files.",
                "http://git-scm.com/docs/git-clean");
            subsec.AddCheat("L", "filter-branch",
                "Removing files from the history: see the link.",
                "http://git-scm.com/en/Git-Tools-Rewriting-History");
        }

        void AddBranching()
        {
            var sec = AddSection("Branching");
            var subsec = sec.AddSubSection("Managing");
            subsec.AddCheat("L", "branch -r | -a",
                "List branches, * means current. -r means show remote tracking branches only, -a means show all " +
                "branches.",
                "http://git-scm.com/docs/git-branch");
            subsec.AddCheat("L", "branch -v | -vv",
                "List branches, -v means show hash and commit subject for each head and the relationship to the " +
                "remote branch, e.g. 'behind 1'. --vv is even nicer, for each tracking branch it shows the name of " +
                "the remote branch beside it.",
                "http://git-scm.com/docs/git-branch");
            subsec.AddCheat("L", "branch -merged",
                "List branches that have been merged into current. Branches without * can be deleted.",
                "http://git-scm.com/docs/git-branch");
            subsec.AddCheat("L", "branch -no-merged", "List branches that have not been merged into current.",
                "http://git-scm.com/docs/git-branch");
            subsec.AddCheat("L", "branch newbrname [sha]",
                "Create a new branch. sha allows you to specify the starting point of the branch and defaults " +
                "to HEAD. This command is not often used, it is easier to do 'git checkout -b newbrname' which " +
                "creates a new branch and immediately switches to it.",
                "http://git-scm.com/docs/git-branch");
            subsec.AddCheat("RLA", "branch --track newbr short/brname",
                "Create a new local branch called 'newbr' to track the branch 'brname' on the remote called " +
                "'short' (which will often be 'origin').",
                "http://git-scm.com/docs/git-branch");
            subsec.AddCheat("L", "branch -d brname",
                "Delete a branch. Normally the branch must be fully merged, but you can use -D to force the delete.",
                "http://git-scm.com/docs/git-branch");
            subsec.AddCheat("L", "show-branch [-r] [-a] [brnames]",
                "Show which commits are in which branches. Defaults to local branches, -r means remote branches " +
                "and -a means all branches. The first section lists each branch (* is current), most recent commit " +
                "and assigns it a column. The second section shows which commits are in which branch: + means it's in, " +
                "* means it's in and this is the active branch, - means it's in and is a merge commit. Try 'gitk --all' " +
                "for a graphical alternative. brnames can include wildcards, for example 'bug/*'.",
                "http://git-scm.com/docs/git-show-branch");


            subsec = sec.AddSubSection("Switching To");
            subsec.AddCheat("LWA", "checkout [-f | -m] brname",
                "Checkout a branch. Your workspace will have files added and removed so that it reflects the " +
                "state of brname. Your index will NOT be affected (in other words, there is only one index, it is " +
                "not per-branch). If your workspace has file differences between the current and new branch then " +
                "the checkout will fail (this is to stop you losing work). Use -f to force it and throw away " +
                "current changes. -m will merge your workspace changes into into brname and leave you on brname (" +
                "YOU MUST RESOLVE THE MERGE INDICATORS IN THE FILE(S)).",
                "http://git-scm.com/docs/git-checkout");
            subsec.AddCheat("L", "checkout -b newbrname [sha]",
                "Create a new branch then check it out. sha allows you to specify the start point of the new " +
                "branch, and defaults to HEAD, in which case because this is a new branch your workspace will not " +
                "be affected. Therefore, this is a way of storing WIP on a new branch.",
                "http://git-scm.com/docs/git-checkout");
            subsec.AddCheat("RLWA", "checkout --track shortname/brname",
                "Create a new local branch (with the same name) to track a remote branch, then switch to it.",
                "http://git-scm.com/docs/git-checkout");
            subsec.AddCheat("RLWA", "checkout -b newname shortname/brname",
                "Create a new local branch (with a new name) to track a remote branch, then switch to it.",
                "http://git-scm.com/docs/git-checkout");


            subsec = sec.AddSubSection("Merging and Rebasing");
            // longest
            subsec.AddCheat("LWA", "merge [--no-commit] sourcebr",
                "Merge sourcebr into the current branch. The changes in sourcebr since it diverged from the " +
                "current branch will be replayed on top of current and then committed (or left in the workspace " +
                "if you specify --no-commit). Merges can fail if there are conflicting changes to a file, in which " +
                "case the commit object is not created and you should run 'git mergetool' to resolve the " +
                "differences. WARNING: Before starting a merge you should commit or stash all workspace changes, " +
                "because it is difficult to back out if there is a merge failure. The --no-commit option is handy " +
                "if you want to review the results of a successful merge before committing it.",
                "http://git-scm.com/docs/git-merge");
            subsec.AddCheat("LWA", "mergetool",
                "Run a merge tool such as Kdiff3 to resolve merge failures. This is only necessary if a merge " +
                "fails. It is usually a good idea to tweak your mergetool, for example to automatically deal with " +
                "whitespace issues.",
                "http://git-scm.com/docs/git-merge");
            subsec.AddCheat("W,NG", "manual resolution",
                "You do not need to run a tool to resolve merge differences. You can just edit the conflicted " +
                "files, 'git add' them, then 'git commit' to complete the process. Be careful to remove any temp " +
                "files that git leaves around, it is easy to accidentally stage them.",
                "http://git-scm.com/docs/git-merge");
            subsec.AddCheat("LWA", "merge --abort",
                "Abort the current conflict resolution process, and try to reconstruct the pre-merge state. This " +
                "might not work unless you committted or stashed everything first.",
                "http://git-scm.com/docs/git-merge");
            subsec.AddCheat("LWA", "rebase [--onto newbase] brname",
                "Rebase first saves all the commits in the current branch that are not in brname to a temporary " +
                "area, then checks out brname and reapplies the commits after the HEAD of brname. The result is " +
                "that N commits are moved from one place in the graph to another. --onto allows you to specify an " +
                "alternative commit (i.e. not HEAD) to apply the rebased commits to. This form is rarely used, but " +
                "the -i form is often used in feature branches for commit squashing. WARNING: Do not rebase " +
                "commits that have been pushed. CAVEAT: Rebasing makes git bisect less useful.",
                "http://git-scm.com/docs/git-rebase");
            subsec.AddCheat("LWA", "rebase -i brname",
                "Interactive rebase. Opens an editor which allows you to specify how to treat each commit in " +
                "the rebase. This is often used to tidy history when using feature or bug branches: leaving the " +
                "line 1 commit as PICK and setting the others to S - SQUASH results in 1 big commit in the " +
                "history rather than N. WARNING: Do not rebase commits that have been pushed. CAVEAT: Rebasing " +
                "makes git bisect less useful.",
                "http://git-scm.com/docs/git-rebase");
            subsec.AddCheat("LWA", "rebase --continue",
                "If a rebase fails with merge conflicts, you can resolve them (using git mergetool or a manual " +
                "process) then issue this command to continue with the rebase.",
                "http://git-scm.com/docs/git-rebase");
            subsec.AddCheat("LWA", "rebase --abort",
                "If a rebase fails with merge conflicts you can issue this command to abort the rebase.",
                "http://git-scm.com/docs/git-rebase");
            subsec.AddCheat("LWA", "checkout --ours | --theirs",
                "VALID DURING A MERGE CONFLICT ONLY. Checkout 'our' version or 'their' version of a file, hence resolving " +
                "the conflict. 'Ours' is defined as the branch you are on, 'theirs' is the version from the branch " +
                "that you are merging in. Obviously, this throws away the changes from one side.",
                "http://git-scm.com/docs/git-checkout");
            subsec.AddCheat("LWA", "cherry-pick sha",
                "Take a single commit and apply it in your current branch. A new commit is created at the tip of your " +
                "branch that contains the state of the original sha. This is another way of copying work from one branch " +
                "to another and is essentially a 'merge of one commit'. Like a full merge, there can be conflicts which " +
                "will require resolution.",
                "http://git-scm.com/docs/git-cherry-pick");
            subsec.AddCheat("L", "merge-base sha1 sha2...",
                "Find the most recent common ancestor of the specified commits. In the simple case of two diverged " +
                "branches it will be the point of divergence, but in more complicated cases such as cross-cutting " +
                "history it can be non-obvious what will be returned. See examples at link.",
                "http://git-scm.com/docs/git-merge-base");


            subsec = sec.AddSubSection("Feature/Bugfix Branch Workflow");
            subsec.AddCheat("LWA", "1. checkout master",
                "Checkout the branch that we want to use as the starting point for our new work.",
                "http://git-scm.com/docs/git-checkout");
            subsec.AddCheat("RLWA", "2. pull",
                "Ensure we have the latest changes from other people.",
                "http://git-scm.com/docs/git-pull");
            subsec.AddCheat("L", "3. checkout -b f-my-feature",
                "Create a new branch to work on new stuff. DO NOT PUSH THIS BRANCH unless other people will need " +
                "to work on it. If you push the branch you won't be able to delete it at the end, though you can " +
                "still rebase commits on the branch as long as they have not been pushed.",
                "http://git-scm.com/docs/git-checkout");
            subsec.AddCheat("WIL,NG", "4. work on your feature",
                "Make as many commits as you like.");
            subsec.AddCheat("RLA", "5. fetch origin (when done)",
                "Get the latest changes from other people into your local repo.",
                "http://git-scm.com/docs/git-fetch");
            subsec.AddCheat("WL", "6. diff origin/master (Optional)",
                "Show the differences between your workspace and the latest changes from others. This can be " +
                "useful during merging/rebasing.",
                "http://git-scm.com/docs/git-diff");
            subsec.AddCheat("LWA", "7. rebase -i origin/master",
                "(Assuming you branched from 'master'). Leave first commit as PICK and set the others to SQUASH, " +
                "this squashes the commit history of the branch f-my-feature down to a single commit which is " +
                "then applied on the end of everybody else's latest work. Use 'git mergetool' to resolve any " +
                "merge conflicts.",
                "http://git-scm.com/docs/git-rebase");
            subsec.AddCheat("LWA", "8. checkout master",
                "Switch to the branch we started from.",
                "http://git-scm.com/docs/git-checkout");
            subsec.AddCheat("RLWA", "9. pull",
                "Updates your workspace (on 'master') with the latest changes from other people.",
                "http://git-scm.com/docs/git-pull");
            subsec.AddCheat("LWA", "10. merge f-my-feature",
                "Merge f-my-feature, which is now a single commit, onto the top of master. Because you already " +
                "rebased relative to the HEAD of master this should be a fast-forward merge, i.e. no extra merge " +
                "commit object will be created. If you do want to create an explicit merge object (say, the " +
                "feature is public and persistent) then use the --no-ff option.",
                "http://git-scm.com/docs/git-merge");
            subsec.AddCheat("L", "11. branch -d f-my-feature",
                "Delete the feature branch, since we no longer need it.",
                "http://git-scm.com/docs/git-branch");
            subsec.AddCheat("LRA", "12. push",
                "Publish your feature to the remote.",
                "http://git-scm.com/docs/git-push");
        }

        void AddRemotes()
        {
            var sec = AddSection("Remotes");
            var subsec = sec.AddSubSection("Setup");
            subsec.AddCheat("L", "remote [-v]",
                "List remotes. Without -v, just displays shortnames. With -v, also displays the urls that the " +
                "shortnames are linked to.",
                "http://git-scm.com/docs/git-remote");
            subsec.AddCheat("L", "remote add shortname url",
                "Add url as a remote repo which can be referenced as 'shortname'. When a repo is created by " +
                "cloning from a url it will already have a remote called 'origin' but you can add others and push " +
                "to them separately.",
                "http://git-scm.com/docs/git-remote");
            subsec.AddCheat("L", "remote set-url shortname url",
                "Change the url that a shortname points to.",
                "http://git-scm.com/docs/git-remote");
            subsec.AddCheat("L", "remote rm shortname",
                "Remove the specified remote.",
                "http://git-scm.com/docs/git-remote");
            subsec.AddCheat("L", "remote rename oldname newname",
                "Change the shortname used to track a remote.",
                "http://git-scm.com/docs/git-remote");
            subsec.AddCheat("L", "remote show shortname",
                "Show info about the remote.",
                "http://git-scm.com/docs/git-remote");


            subsec = sec.AddSubSection("Fetching and Pulling");
            subsec.AddCheat("RLA", "fetch [shortname]",
                "Fetch all commits for remote shortname (defaults to origin) into your local repo. This includes " +
                "new remote branches, but new local tracking branches will not be created; if you " +
                "'git checkout br'  a new local branch will be created and automatically linked to the remote " +
                "branch of the same name. Use 'git branch -a' to list all branches.",
                "http://git-scm.com/docs/git-fetch");
            subsec.AddCheat("RLA", "fetch --tags [shortname]",
                "Fetch all tags from the remote shortname. In git versions prior to v1.9 this does NOT fetch " +
                "everthing a normal fetch does, it just gets tags. For versions > 1.9, it DOES fetch everything a " +
                "normal fetch does and hence is the only command you need.",
                "http://git-scm.com/docs/git-fetch");
            // longest
            subsec.AddCheat("RLWA", "pull [--rebase] [shortname]",
                "Fetch all commits from remote repo into your local repo then on your CURRENT BRANCH ONLY merge " +
                "the fetched commits into your workspace. The commits of other branches will not be merged, which " +
                "is why you get the message 'Your branch is behind origin/master...' when you switch branches. " +
                "n.b. It is NOT a good idea to try and write a function to merge all branches automatically " +
                "because some might fail and you will be left with a mess. Pull is effectively a git fetch " +
                "followed by a git merge. Many people prefer to use the --rebase option instead of doing a merge " +
                "as it results in a simpler commit history.",
                "http://git-scm.com/docs/git-pull");


            subsec = sec.AddSubSection("Manual Fetch and Merge");
            subsec.AddCheat("RLA", "fetch shortname",
                "Fetch the latest updates from the remote specified by shortname.",
                "http://git-scm.com/docs/git-fetch");
            subsec.AddCheat("WL", "diff master shortname/master",
                "Show the differences between your local tracking branch 'master' and the remote 'master'. " +
                "That is, show what changes other people have made, before the changes hit your workspace.");
            subsec.AddCheat("LWA", "merge shortname/master",
                "Merge other people's changes into your workspace.",
                "http://git-scm.com/docs/git-merge");
            subsec.AddCheat("LWA", "rebase [-i] shortname/master",
                "Rebase other people's changes into your workspace. The rebase backs out all the commits you have " +
                "made since the point of divergence, sets HEAD to the tip of origin/master, then reapplies your " +
                "changes on top. Hence, in this usage, it basically means 'accept everything that everybody else " +
                "has done then apply my stuff on top'.",
                "http://git-scm.com/docs/git-rebase");


            subsec = sec.AddSubSection("Pushing");
            subsec.AddCheat("LRA", "push",
                "Push all commits on branches that exist in BOTH the local and remote repo, i.e. remote branches " +
                "for which you have a local tracking branch. See following commands for ways to push new branches, " +
                "however often you do NOT want to push all your local branches, especially when working with " +
                "temporary bug or feature branches.",
                "http://git-scm.com/docs/git-push");
            subsec.AddCheat("LRA", "push -u shortname brname",
                "Push brname to the remote repo (creating it if necessary) and (-u) create a local tracking " +
                "branch so that in future you can just do 'git push'.",
                "http://git-scm.com/docs/git-push");
            subsec.AddCheat("LRA", "push --all -u shortname",
                "Same as previous command, but --all means push all locally created branches. Use sparingly, " +
                "other people propbably aren't interested in all your many temporary branches.",
                "http://git-scm.com/docs/git-push");
            subsec.AddCheat("LRA", "push shortname brname:brname2",
                "Push brname to shortname, renaming it brname2 on the remote.",
                "http://git-scm.com/docs/git-push");
            subsec.AddCheat("LRA", "push shortname :brname",
                "Delete the brname branch from the remote.",
                "http://git-scm.com/docs/git-push");
            subsec.AddCheat("LRA", "push shortname v1.5",
                "Push a tag to a remote (tags are not pushed by default).",
                "http://git-scm.com/docs/git-push");
            subsec.AddCheat("LRA", "push shortname --tags",
                "Push all tags to a remote (tags are not pushed by default).",
                "http://git-scm.com/docs/git-push");


            subsec = sec.AddSubSection("Subtrees (Sharing code)");
            subsec.AddCheat(",NG", "Introduction",
                "Subtrees supercede submodules. The usage below shows the most common case: " +
                "setting up a MAIN repo that contains a sub-repo (e.g. for common code) called MiscUtils. " +
                "It is a good idea to setup aliases called mupull and mupush in MAIN's .git/config. The 'mu' " +
                "prefix allows you to have several different subtrees, just setup a different alias for each one. " +
                "More advanced usages, such as initially splitting out a subproject, are possible, see the " +
                "documentation at the link and google.",
                "https://github.com/git/git/blob/master/contrib/subtree/git-subtree.txt");
            subsec.AddCheat("RLWA", "subtree add ...",
                "Full command: git subtree add --prefix MiscUtils [--squash] remote master. Fetches the 'remote' " +
                "repo and checks out its files (as specified by the ref 'master') into the MiscUtils folder. " +
                "For consistency it is a good idea to run all subtree commands from the root of MAIN. You don't " +
                "need an alias for this command because you only need to run it once. --squash squashes all the " +
                "commits in MiscUtils down to a single commit before merging - this keeps MAIN's commit history " +
                "cleaner. Look in git log to see what happened.",
                "https://github.com/git/git/blob/master/contrib/subtree/git-subtree.txt");
            subsec.AddCheat("RLWA", "subtree pull ...",
                "Full command: git subtree pull --prefix MiscUtils [--squash] remote master. Fetches the latest changes " +
                "from the remote sub-repo and then merges them into your MAIN repo. --squash squashes all the " +
                "commits in MiscUtils down to a single commit before merging - this keeps MAIN's commit history cleaner.",
                "https://github.com/git/git/blob/master/contrib/subtree/git-subtree.txt");
            subsec.AddCheat("LRA", "subtree push ...",
                "Full command: git subtree push --prefix MiscUtils remote master. Push changes that you have made " +
                "in your local copy of MiscUtils back up the remote repo. In other words, this is how you contribute " +
                "code back to the master MiscUtils repository. It is advisable to pull the latest remote changes down " +
                "first before trying to push your stuff back up. n.b. See the advice at the link about splitting out " +
                "commits that affect subtrees; it's not necessary but is a good idea.",
                "https://github.com/git/git/blob/master/contrib/subtree/git-subtree.txt");
            subsec.AddCheat(",NG", "Terminating the link",
                "Nothing to do. Subtrees don't use any special files or tracking, so just stop using 'subtree pull' " +
                "and 'subtree push'.",
                "https://github.com/git/git/blob/master/contrib/subtree/git-subtree.txt");
        }

        void AddReviewing()
        {
            var sec = AddSection("Reviewing");
            var subsec = sec.AddSubSection("Comparing");
            subsec.AddCheat("WI", "diff [-- path] [--stat]",
                "Compare the workspace to the index. This form shows the unstaged changes. --stat draws the file " +
                "modification histogram and can be used with any diff command.",
                "http://git-scm.com/docs/git-diff");
            subsec.AddCheat("IL", "diff --staged [sha] [-- path]",
                "Compare the index to the repo. sha defaults to HEAD, therefore this form shows the change you " +
                "are about to commit. Use a branch name to compare to the tip of that branch. --cached is a " +
                "synonym for --staged.",
                "http://git-scm.com/docs/git-diff");
            subsec.AddCheat("WL", "diff sha [-- path]",
                "Compare the workspace to the repo. sha will typically be HEAD, therefore this form shows your " +
                "current state vs the last commit.",
                "http://git-scm.com/docs/git-diff");
            subsec.AddCheat("L", "diff sha1 sha2 [-- path]",
                "Compare two arbitrary commits. The index and workspace are not involved. You may omit one commit, " +
                "which will default to HEAD. n.b. sha1 and sha2 can be branch names, which is how you compare a " +
                "file across branches. You can use the alternative double-dot syntax, sha1..sha2, which means " +
                "exactly the same thing (diff the two commits). This is very different from the use of .. " +
                "in the log command.",
                "http://git-scm.com/docs/git-diff");
            subsec.AddCheat("L", "diff HEAD~ -- filespec",
                "Example of the above: compare the most recently committed version of filespec with the version " +
                "prior to that ('show me the change I just committed').",
                "http://git-scm.com/docs/git-diff");
            subsec.AddCheat("L", "diff sha1\\...sha2 [-- path]",
                "View the changes on the branch containing the second commit, starting at the most recent common " +
                "ancestor of both commits. Either commit can be omitted and defaults to HEAD.",
                "http://git-scm.com/docs/git-diff");
            subsec.AddCheat("L", "diff --name-status --oneline sha1 sha2",
                "Get a list of files changed between two commits with an indicator of what happened. This is a more " +
                "exotic version of the simple 'diff --name-only' command, which just gives you the file list (handy " +
                "for scripting).",
                "http://git-scm.com/docs/git-diff");
            subsec.AddCheat("L", "diff :1:filespec :3:filespec",
                "VALID DURING A MERGE ONLY. Compare the mergebase (1) to 'theirs' (3). In other words, this shows you " +
                "the changes made on the branch that you are merging in since it diverged. 2 is 'our' version.",
                "http://git-scm.com/docs/git-diff");
            subsec.AddCheat(null, "difftool",
                "Run a graphical diff, which you must first have configured in .gitconfig. It accepts the same " +
                "arguments as git-diff. Be careful not to diff binary files.",
                "http://git-scm.com/docs/git-difftool");


            subsec = sec.AddSubSection("Status and History");
            subsec.AddCheat("WIL", "status [--short] [--ignored]",
                "Display status of files in the workspace and index vs the repo. --short is very brief, see the " +
                "link for explanation. --ignored is a good way of finding the ignored files.",
                "http://git-scm.com/docs/git-status");
            subsec.AddCheat("L,NG", "gitk [--all] [file]",
                "Display graphical repository browser. Accepts the same options as " +
                "'git log'. The --all option makes it display the branching history for every branch, otherwise " +
                "it just shows your current branch. Pass a file to look at the history of an individual file.",
                "http://git-scm.com/docs/gitk");
            subsec.AddCheat("L", "log [options] revisionrange",
                "Limit commits to those in the range, which defaults to HEAD, i.e. everything. 'origin..HEAD' " +
                "means all commits reachable from the current commit (i.e. HEAD) but not from origin. So for " +
                "example if you are on a branch forked from master, 'master..' will limit to the commits on that " +
                "branch.",
                "http://git-scm.com/docs/git-log");
            subsec.AddCheat("L", "log [options] brname",
                "Example of the previous: show the log of a particular branch.",
                "http://git-scm.com/docs/git-log");
            subsec.AddCheat("L", "log [options] -- pathspec",
                "Restrict history to those commits affecting file(s) or folder(s). Pathspec is always last, after " +
                "the revisionrange. The full format is not always necessary, for example 'git log README.md' " +
                "will work.",
                "http://git-scm.com/docs/git-log");
            subsec.AddCheat("L", "log [-N]",
                "N = number of commits to limit to.",
                "http://git-scm.com/docs/git-log");
            subsec.AddCheat("L", "log --after=datespec",
                "Only show commits after datespec. You can also use 'before'. datespec can be '2.weeks', " +
                " '3 months ago' (in quotes), a date literal such as '27-09-2013' or even just a time such " +
                "as '14:30'.",
                "http://git-scm.com/docs/git-log");
            subsec.AddCheat("L", "log -p",
                "Display log with diffs.",
                "http://git-scm.com/docs/git-log");
            subsec.AddCheat("L", "log --stat | --shortstat",
                "Display log with statistics. --stat shows the name of each file with lines inserted and deleted in a " +
                "histogram. --shortstat only shows the last line, the one that says " +
                "'2 files changed, 3 insertions(+), 1 deletion(-)'.",
                "http://git-scm.com/docs/git-log");
            subsec.AddCheat("L", "log --author=somebody",
                "Show commits authored by 'somebody' (which is a regex). The author is the person who originally " +
                "wrote the code. The match is against the 'Author:' line that is printed by the git log command. " +
                "'git log --author=Ph' or 'git log --author=Da' is sufficient to find commits by 'Philip " +
                "Daniels', you can use parts of the email address as well.",
                "http://git-scm.com/docs/git-log");
            subsec.AddCheat("L", "log --committer=somebody",
                "Show commits committed by 'somebody' (which is a regex). The committer is the person who applied " +
                "the patch. Normally committer and author are the same, the committer is only different if you " +
                "are sending patches off to someone.",
                "http://git-scm.com/docs/git-log");
            subsec.AddCheat("L", "log --grep=pattern",
                "Show commits with messages that match the pattern.",
                "http://git-scm.com/docs/git-log");
            subsec.AddCheat("L", "log -S'string'",
                "Show commits that add or remove the string in the content. Example: git log -S'void foo()'.",
                "http://git-scm.com/docs/git-log");
            subsec.AddCheat("L", "log -G'pattern'",
                "Show commits that add or remove the pattern in the content. Example git log -G'^int gtk'.",
                "http://git-scm.com/docs/git-log");
            subsec.AddCheat("L", "log -L start,end:file",
                "Show the commits that affected the line range start-end within 'file'. start and end can be " +
                "numbers, regexes or offsets (see the documentation, there is also a good example at the bottom). " +
                "This is a great way of finding what changed something in a file.",
                "http://git-scm.com/docs/git-log");
            subsec.AddCheat("L", "log -i",
                "The -i option makes pattern matches for --author, --committer, --grep etc. case-insensitive.",
                "http://git-scm.com/docs/git-log");
            subsec.AddCheat("L", "log --all-match",
                "AND all match options together (default is OR).",
                "http://git-scm.com/docs/git-log");
            subsec.AddCheat("L", "log origin/master..HEAD",
                "Show commits in current branch that aren't at the remote (i.e. will be pushed).",
                "http://git-scm.com/docs/git-log");
            subsec.AddCheat("L", "log --pretty=...",
                "Specify log format. Valid options are short, medium, full, fuller, email, raw and format:<string>",
                "http://git-scm.com/docs/git-log");
            subsec.AddCheat("L", "blame filespec",
                "Show which commit and person was responsible for each line of a file.",
                "http://git-scm.com/docs/git-blame");
            subsec.AddCheat("L", "gui blame filespec", "Graphical blame.",
                "http://git-scm.com/docs/git-gui");


            subsec = sec.AddSubSection("Commit Ranges");
            subsec.AddCheat("L", "log sha",
                "Include commits that are reachable from (i.e. ancestors of) sha.",
                "http://git-scm.com/docs/gitrevisions.html");
            subsec.AddCheat("L", "log ^sha",
                "Exclude commits that are reachable from (i.e. ancestors of) sha.",
                "http://git-scm.com/docs/gitrevisions.html");
            subsec.AddCheat("L", "log sha1..sha2",
                "Include commits that are reachable from sha2 but exclude those that are reachable from sha1. Mnemonic: " +
                "'In end but not start.' This is exactly the same as the longhand form 'log ^sha1 sha2'. When either sha1 " +
                "or sha2 is omitted, it defaults to HEAD.",
                "http://git-scm.com/docs/gitrevisions.html");
            subsec.AddCheat("L", "log master..f-myfeature",
                "Example: show commits in branch f-myfeature that are not in master.",
                "http://git-scm.com/docs/gitrevisions.html");
            subsec.AddCheat("L", "log f-myfeature..master",
                "Example: show commits in master that are not in your branch (useful for previewing merges).",
                "http://git-scm.com/docs/gitrevisions.html");
            subsec.AddCheat("L", "log origin/master..HEAD",
                "Example: show commits that are not pushed to the remote.",
                "http://git-scm.com/docs/gitrevisions.html");
            subsec.AddCheat("L", "log ^exc1 ^exc2 inc1 inc2",
                "The general form of 'git log' commit ranges. Specifying a commit means 'show me that commit and its " +
                "ancestors'. Negating that with ^ means '...but exclude this commit and its ancestors.' ",
                "http://git-scm.com/docs/gitrevisions.html");
        }

        void AddMisc1()
        {
            var sec = AddSection("Misc 1");
            var subsec = sec.AddSubSection("Showing Objects");
            subsec.AddCheat("L", "revparse ref",
                "Show the sha that 'ref' refers to. ref may be a branch name, a tag, or any other type of reference.",
                "http://git-scm.com/docs/git-rev-parse");
            subsec.AddCheat("L", "show [options] [things]",
                "Show details of things (blobs, trees, tags and commits). Examples of naming objects " +
                "are covered in the next examples, which use git-show, but link to the gitrevisions page where " +
                "revisions are described in detail.",
                "http://git-scm.com/docs/git-show");
            subsec.AddCheat("L", "show sha1",
                "Show details of specified commit (defaults to HEAD).",
                "http://git-scm.com/docs/gitrevisions.html");
            subsec.AddCheat("L", "show ref",
                "Ref is a symbolic name, such as master, heads/master or refs/heads/master.",
                "http://git-scm.com/docs/gitrevisions.html");
            subsec.AddCheat("L", "show ref:pathspec",
                "Show the contents of the file in the ref. For example, ref may be a branch name or HEAD~10. " +
                "Therefore 'show dev~5:Makefile' shows what Makefile looks like on the dev branch 5 revisions ago.",
                "http://git-scm.com/docs/gitrevisions.html");
            subsec.AddCheat("I", "show :pathspec",
                "Show a file as it exists in the index.",
                "http://git-scm.com/docs/gitrevisions.html");
            subsec.AddCheat("L", "show @", "@ by itself is a shortcut for HEAD.",
                "http://git-scm.com/docs/gitrevisions.html");
            subsec.AddCheat("L", "show ref@{date}",
                "A point in time, e.g. 'master@{yesterday}' shows the master branch as it appeared yesterday, " +
                "'HEAD@{5 minutes ago}' shows the current branch as it was 5 minutes ago. NOTE: This is not as " +
                "useful as it appears because it only works on things that are in your local reflog ('git " +
                "reflog'), so it is no good on a repository you have just cloned. Use --before and --after instead.",
                "http://git-scm.com/docs/gitrevisions.html");
            subsec.AddCheat("L", "show ref@{N}",
                "N is a number. master@{1} means the immediate prior value of master, while master@{5} means " +
                "the 5th prior value. This is also dependent upon the git reflog.",
                "http://git-scm.com/docs/gitrevisions.html");
            subsec.AddCheat("L", "show rev~N",
                "N is a number and means 'Nth ancestor', i.e. follow the commit chain back in time always using " +
                "the first parent. ~ therefore allows you to examine the history of a file. It is quite different " +
                "to ^N, which is only useful for merges.",
                "http://git-scm.com/docs/gitrevisions.html");
            subsec.AddCheat("L", "show rev^N",
                "N is a number and means 'Nth parent'. Normally N is omitted and defaults to 1, which means " +
                "'the first parent'. ONLY MERGES HAVE MORE THAN ONE PARENT.",
                "http://git-scm.com/docs/gitrevisions.html");
            subsec.AddCheat("L", "show HEAD~4:File > OldFile",
                "An example of how to recover an old version of a file into a new filename. If you just want to " +
                "replace in your workspace, you can do a 'git checkout' instead.",
                "http://git-scm.com/docs/gitrevisions.html");


            subsec = sec.AddSubSection("Stashing");
            subsec.AddCheat("S", "stash list",
                "Show stack of stashes. stash@{0} is the latest stash. The branch that was current when the " +
                "stash was made is also shown.",
                "http://git-scm.com/docs/git-stash");
            subsec.AddCheat("IWSA", "stash save [--untracked | --all] [msg]",
                "Copy the current state of the workspace and index into a new stash then run 'git reset --hard' " +
                "to undo your changes (if there are no changes this is a no-op). --untracked will also include " +
                "UNTRACKED files in the stash and then delete them using 'git clean' (you should probably do " +
                "this), and --all will also include IGNORED files the stash. This is only necessary if you are " +
                "planning on changing what is .gitignored.",
                "http://git-scm.com/docs/git-stash");
            subsec.AddCheat("S", "stash show [stash]",
                "Show the difference between the stash and its parent, i.e. this shows the change that the " +
                "stash introduces.",
                "http://git-scm.com/docs/git-stash");
            subsec.AddCheat("SWA", "stash pop [stash]",
                "Apply the specified stash (defaults to stash@{0}) then delete it. If the application fails " +
                "due to conflicts then resolve them manually then do 'git stash drop' to remove the stash manually.",
                "http://git-scm.com/docs/git-stash");
            subsec.AddCheat("SWA", "stash apply [stash]",
                "Like pop, but does not delete the stash when done.",
                "http://git-scm.com/docs/git-stash");
            subsec.AddCheat("SWLA", "stash branch brname [stash]",
                "Create and checkout brname starting from the commit at which the stash was originally created, " +
                "apply the stash on top then drop the stash. This effectively converts a stash into a branch.",
                "http://git-scm.com/docs/git-stash");
            subsec.AddCheat("S", "stash drop [stash]",
                "Drop a stash.",
                "http://git-scm.com/docs/git-stash");
            subsec.AddCheat("S", "stash clear",
                "Drop all stashes.",
                "http://git-scm.com/docs/git-stash");


            subsec = sec.AddSubSection("Tagging");
            subsec.AddCheat("L,NG", "tags vs branches",
                "A tag and a branch serve different purposes. A tag is meant to be a static name that does not change or " +
                "move over time. Once applied, you should leave it alone. It serves as a stake in the ground and a " +
                "reference point. On the other hand, a branch is dynamic and moves with each commit you make. The branch " +
                "name is designed to follow your continuing development. - Jon Loeliger, Version Control with Git.",
                null);
            subsec.AddCheat("L", "tag",
                "List all tags.",
                "http://git-scm.com/docs/git-tag");
            subsec.AddCheat("L", "tag -l v1.4*",
                "List all tags matching pattern.",
                "http://git-scm.com/docs/git-tag");
            subsec.AddCheat("L", "show v1.4",
                "Show the tag message, the commit message, and a diff of the commit.",
                "http://git-scm.com/docs/git-tag");
            subsec.AddCheat("L", "tag -a v1.4 -m \"message\"",
                "Create the annotated tag 'v1.4' with specified message. You should always use annotated tags and " +
                "avoid lightweight tags.",
                "http://git-scm.com/docs/git-tag");
            subsec.AddCheat("L", "tag -a v1.4 sha",
                "Tag a specific commit, can be after the fact.",
                "http://git-scm.com/docs/git-tag");
            subsec.AddCheat("LRA", "push shortname v1.5",
                "Push a tag to a remote (tags are not pushed by default).",
                "http://git-scm.com/docs/git-tag");
            subsec.AddCheat("LRA", "push origin --tags",
                "Push all tags to a remote (tags are not pushed by default).",
                "http://git-scm.com/docs/git-tag");
            subsec.AddCheat("L", "tag -f v1.4 sha",
                "Force a tag creation on a particular commit. You can use this to move a tag from one commit to " +
                "another BUT DON'T DO THIS if you have pushed the tag (see discussion at link).",
                "http://git-scm.com/docs/git-tag");
            subsec.AddCheat("L", "tag -d v1.4", "Delete a tag.",
                "http://git-scm.com/docs/git-tag");


            subsec = sec.AddSubSection("Bisection");
            subsec.AddCheat("LWA", "bisect start",
                "Bisect uses a binary search through your commit history to find a bug. Use 'bisect start' to start " +
                "the process, then 'bisect good' and 'bisect bad' to set the bounds of the search. See link for " +
                "more details.",
                "http://git-scm.com/docs/git-bisect");
            subsec.AddCheat("LWA", "bisect run my_script args",
                "If you have a script (such as a compile & unit test script) that can return a code indicating whether " +
                "a build is good or bad you can automate the entire process.",
                "http://git-scm.com/docs/git-bisect");

            subsec = sec.AddSubSection("Plumbing Commands");
            subsec.AddCheat("L", "ls-files -s | -u",
                "Show staged files. -s shows all staged files, -u shows just those with conflicts. This can be useful " +
                "during a merge to examine each version. The numbers are stage numbers. 1 is the merge base, 2 is 'our' " +
                "version, and 3 is 'their' version. (0 means a non-conflicted staged file). Use git cat-file to examine " +
                "each file. If you are on master and merging in brname, then 'ours' is the master version, and 'theirs' " +
                "is the brname version.",
                "http://git-scm.com/docs/git-ls-files");
            subsec.AddCheat("L", "cat-file -p sha",
                "Cat a file. This low-level command is useful for examining the 3 versions of a file during merge conflicts.",
                "http://git-scm.com/docs/git-cat-file");
            subsec.AddCheat("L", "reflog",
                "Display the reflog, a history of changes to refs within your repo.",
                "http://git-scm.com/docs/git-reflog");
        }
    }
}

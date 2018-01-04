// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// Import the utility functionality.

import jobs.generation.ArchivalSettings;
import jobs.generation.Utilities;

def project = GithubProject
def branch = GithubBranchName

def static getBuildJobName(def configuration, def os) {
    return configuration.toLowerCase() + '_' + os.toLowerCase()
}

['Windows_NT', 'Ubuntu14.04', 'OSX10.12'].each { os ->
    ['Debug', 'Release'].each { config ->
        [true, false].each { isPR ->
            // Calculate job name
            def jobName = getBuildJobName(config, os)
            def buildCommand = '';

            def osBase = os
            def machineAffinity = 'latest-or-auto'

            // Calculate the build command
            if (os == 'Windows_NT') {
                buildCommand = ".\\build\\cibuild.cmd -configuration $config"
                machineAffinity = 'latest-dev15-3'
            }
            else {
                buildCommand = "./build.sh --configuration $config"
            }

            def newJob = job(Utilities.getFullJobName(project, jobName, isPR)) {
                // Set the label.
                steps {
                    if (osBase == 'Windows_NT') {
                        // Batch
                        batchFile(buildCommand)
                    }
                    else {
                        // Shell
                        shell(buildCommand)
                    }
                }
            }

            Utilities.setMachineAffinity(newJob, osBase, machineAffinity)
            Utilities.standardJobSetup(newJob, project, isPR, "*/${branch}")

            if (isPR) {
                Utilities.addGithubPRTriggerForBranch(newJob, branch, "$os $config")
            }

            def archiveSettings = new ArchivalSettings()
            archiveSettings.addFiles("artifacts/$config/log/*")
            archiveSettings.addFiles("artifacts/$config/TestResults/*")
            archiveSettings.setFailIfNothingArchived()
            archiveSettings.setArchiveOnFailure()
            Utilities.addArchival(newJob, archiveSettings)
        }
    }
}

def version = '0.0.0-UNKNOWN'

def gitUrl = 'git@bitbucket.org:htl-md/contentservice.git'
def dockerImageName = 'tuwelsolution'
def dllName = 'TuwelSolution.dll'
def hasPackage = false

podTemplate(yaml:'''
              spec:
                serviceAccountName: jenkins
                containers:
                - name: jnlp
                  image: repo.htl-md.schule:5003/jenkins/inbound-agent:4.7-1
                  volumeMounts:
                  - name: home-volume
                    mountPath: /home/jenkins
                  env:
                  - name: HOME
                    value: /home/jenkins
                - name: dotnet6
                  image: mcr.microsoft.com/dotnet/sdk:6.0
                  command:
                  - sleep
                  args: 
                  - 99d
                  volumeMounts:
                  - name: home-volume
                    mountPath: /home/jenkins
                  env:
                  - name: HOME
                    value: /home/jenkins
                - name: docker
                  image: repo.htl-md.schule:5003/docker:19.03.1
                  command:
                  - sleep
                  args:
                  - 99d
                  volumeMounts:
                  - name: docker-socket
                    mountPath: /var/run
                  - name: home-volume
                    mountPath: /home/jenkins
                - name: docker-daemon
                  image: repo.htl-md.schule:5003/docker:19.03.1-dind
                  securityContext:
                    privileged: true
                  volumeMounts:
                  - name: docker-socket
                    mountPath: /var/run
                volumes:
                - name: home-volume
                  emptyDir: {}
                - name: docker-socket
                  emptyDir: {}
''') {

    node(POD_LABEL) {
        stage('Pull Git Repo') {
            git url: gitUrl, 
                credentialsId: 'bitbucket-schooldirector'
            container('dotnet6') {
                stage('Set Project version') {
                    script {
                        //Get latest tag on current branch
                        def tag = sh label:'Calculate project version from Tag', returnStdout: true,
                                        script: 'git describe --tags $(git rev-list --tags --max-count=1)'
                        //Append the Build-number to the tag
                        version = tag.trim() + "." + env.BUILD_NUMBER
                        
                        //Replace the placeholder with the actual version
                        sh label: 'Apply Project version',
                           script: "sed -i 's/{CI_VERSION}/$version/g' **/*.csproj"  
                   }      
                }
                stage('Build Project') {
                    sh label: 'Restoring nuget packages',
                       script: 'dotnet restore -s https://repo.htl-md.schule/repository/nuget-group/index.json'
                    sh label: 'Build solution',
                       script: 'dotnet publish --no-restore -c Release -o out'
                }
                if (hasPackage) {
                    stage ('Publish Project') {
                        withCredentials([usernamePassword(credentialsId: 'nexus-deploybot-nuget', passwordVariable: 'apiKey', usernameVariable: 'user')]) {
                          sh label: 'Push to nuget repository',
                             script: 'dotnet nuget push "**/*.nupkg" -k $apiKey -s https://repo.htl-md.schule/repository/nuget-hosted/'
                        }
                    }
                }
            }
            container('docker') {
                stage ('Build and push docker image') {
                    sh label: 'Build docker image',
                       script: "docker build --build-arg DLL_NAME=$dllName . -t repo.htl-md.schule:5004/$dockerImageName:v$version -t repo.htl-md.schule:5004/$dockerImageName:latest -t htlmd/$dockerImageName:v$version"
                    withCredentials([usernamePassword(credentialsId: 'nexus-deploybot', passwordVariable: 'password', usernameVariable: 'user')]) {
                          sh label: 'Login to docker repo',
                             script: 'docker login --username $user --password $password repo.htl-md.schule:5004'
                    
                          sh label: 'Push docker image',
                             script: "docker push repo.htl-md.schule:5004/$dockerImageName:v$version"
                    }
                }
            }
        }
    }
}
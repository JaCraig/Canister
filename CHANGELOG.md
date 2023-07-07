# Change Log

All notable changes to this project will be documented in this file. See [versionize](https://github.com/versionize/versionize) for commit guidelines.

<a name="5.1.1"></a>
## [5.1.1](https://www.github.com/JaCraig/Canister/releases/tag/v5.1.1) (2023-7-7)

### Bug Fixes

* Updating nuget package with final info ([389fc29](https://www.github.com/JaCraig/Canister/commit/389fc29c437650897eab1658f5884ebc65f069e8))

<a name="5.1.0"></a>
## [5.1.0](https://www.github.com/JaCraig/Canister/releases/tag/v5.1.0) (2023-7-7)

### Features

* Adding the ability to do generic classes with AddAll extension methods ([9b1670d](https://www.github.com/JaCraig/Canister/commit/9b1670decb7bf4566a6b0a38dff8d39717c2d54b))

<a name="5.0.1"></a>
## [5.0.1](https://www.github.com/JaCraig/Canister/releases/tag/v5.0.1) (2023-7-7)

### Bug Fixes

* Adding automation, documentation, and example app ([ab4f3b4](https://www.github.com/JaCraig/Canister/commit/ab4f3b490a414aed39f995786f5a9b7405ad312e))

### Other

*  ([a802ca5](https://www.github.com/JaCraig/Canister/commit/a802ca5d558dc62c0d6fb540a0cc6724c874a82e))
*  ([674cb93](https://www.github.com/JaCraig/Canister/commit/674cb93644919656dde8257ddcf6e951e02b74df))
*  ([def15f3](https://www.github.com/JaCraig/Canister/commit/def15f3bff09620c23e5a616bc84e1e442f2c59b))
*  ([c34b72b](https://www.github.com/JaCraig/Canister/commit/c34b72b246d40f74ef12b538188d99935b1789e4))
*  ([254eff1](https://www.github.com/JaCraig/Canister/commit/254eff121aa6a28b50e499fd206352ee0d1ade47))
*  ([98f2d95](https://www.github.com/JaCraig/Canister/commit/98f2d95936980b2eec0b23ef53aa050c1ae68b8e))
* -  Merge. ([59d6070](https://www.github.com/JaCraig/Canister/commit/59d60706618dc3d0f34f516a461c6e8e7d8cc48d))
* - Adding some testing. ([1b494ec](https://www.github.com/JaCraig/Canister/commit/1b494ec0d7e51ca0a77ae8b26c1548cb005214de))
* - Apparently package updates didn't take? ([1740724](https://www.github.com/JaCraig/Canister/commit/1740724bbc001d83c7ee3682fc89bdafce33ed53))
* - Final merge changes. ([d6303dc](https://www.github.com/JaCraig/Canister/commit/d6303dc3465765918fb166e575fd47ed0f6259d7))
* - Fix for issue when trying to load modules from test framework. ([d69d6b7](https://www.github.com/JaCraig/Canister/commit/d69d6b7c2e41f811cdf722b9a5240099d8dd5d89))
* - Fix for service collection extension. ([8de344f](https://www.github.com/JaCraig/Canister/commit/8de344f36aa212da3b8ef2885a9948de9c27face))
* - Fixing potential issues if using the Canister.Builder.Bootstrapper property and race conditions. ([8b1b9b7](https://www.github.com/JaCraig/Canister/commit/8b1b9b7a702336730eeddabcc1648e1ac385fe24))
* - Large switch. System is now just a set of extension methods on top of IServiceCollection. Any usage will require a rebuild, hence bump to version 5. ([28ca42d](https://www.github.com/JaCraig/Canister/commit/28ca42dbbea9892fed55d2c576f3e033d5138029))
* - Package updates. ([c618a57](https://www.github.com/JaCraig/Canister/commit/c618a57585194522e764cc201db9871425af2eea))
* - Sealing default boot strapper. ([66b2d00](https://www.github.com/JaCraig/Canister/commit/66b2d00f7567da502cbdf7f2ffa523ef247cec3f))
* Added a basic benchmark test for testing ConstructorInfo service vs Factory. Was able to get it down to about equal on speed for transient and scoped objects. ([6b9033b](https://www.github.com/JaCraig/Canister/commit/6b9033b626c7b0a665e28f302b94d9df683812da))
* Added test app for 3.0 integration. Added CanisterServiceProviderFactory to work with the new IoC setup. ([3243407](https://www.github.com/JaCraig/Canister/commit/32434075c1f8718f30194b3be85554644151f7df))
* Added the basics for the lifetime classes and service types. Basically breaking apart the TypeBuilder classes a bit more. ([178c409](https://www.github.com/JaCraig/Canister/commit/178c409e8f3318940af366bd8b08987b77ab39aa))
* Adding basic module to register default types (pretty much just strings at the moment). ([dbe2518](https://www.github.com/JaCraig/Canister/commit/dbe2518ef1305e90f65dcff4242f0f10a42bc9cd))
* Adding files for the default IoC container and builder/bootstrapper code. ([e618174](https://www.github.com/JaCraig/Canister/commit/e618174f16039deaac772377f7c7ef06a6fb33d3))
* Adding some code in the constructor service for dealing with value types. ([2129070](https://www.github.com/JaCraig/Canister/commit/21290701dab80697f23723b75622a2bb7b234412))
* Adding source link. ([9d1cc10](https://www.github.com/JaCraig/Canister/commit/9d1cc104b8ad3a50a3bb0273c2fb0ce0f10bff70))
* Basic cleanup. ([62dfa51](https://www.github.com/JaCraig/Canister/commit/62dfa5126df8b0ed309b23437ecef14f21148b60))
* Bump Microsoft.AspNetCore.All from 2.1.0 to 2.1.4 in /SimpleMVCTests ([ea27036](https://www.github.com/JaCraig/Canister/commit/ea27036406f6fb936f9a35b16e099708d342789b))
* Bump Microsoft.AspNetCore.All from 2.1.4 to 2.1.15 in /SimpleMVCTests ([766a241](https://www.github.com/JaCraig/Canister/commit/766a241a0fc74e078639bc7da612f1642b4f1cc9))
* Changes to Canister in terms of how it loads. Can either do ServiceProviderFactory or extension method on IServiceCollection. ([e0f2e47](https://www.github.com/JaCraig/Canister/commit/e0f2e47d91a4a7643969575a3fab3677ba117817))
* Create codeql-analysis.yml ([a6dd051](https://www.github.com/JaCraig/Canister/commit/a6dd051c6cb293312846e4f17d30d359408ef774))
* Create dependabot.yml ([3265004](https://www.github.com/JaCraig/Canister/commit/32650040205ec3d7ef09fbb8145f8a079accbb64))
* Documentation update and version update. ([c3e3f43](https://www.github.com/JaCraig/Canister/commit/c3e3f4319d743095a7a596cdeb1d79644e37bd0d))
* Finished with basic transition of the TypeBuilder to Services/Lifetime code. ([46997f6](https://www.github.com/JaCraig/Canister/commit/46997f66d04b895b1da88b62e8508a4a2dfe1040))
* Fix for default value on optional parameters. ([5b81ad6](https://www.github.com/JaCraig/Canister/commit/5b81ad65dd20d35b76457f321368e59406f68eae))
* Fixed issues with the bootstrapper not being registered properly. ([635f07c](https://www.github.com/JaCraig/Canister/commit/635f07c1412712f6edc6c1158dfd8f78b92d9078))
* Fixing issue if the assembly does not have any defined types. ([915140f](https://www.github.com/JaCraig/Canister/commit/915140f73dd4a7d0a6a1ab7e64ed4ac1b725a0d0))
* Fixing issue when the bootstrapper is requested as a parameter in a constructor when resolving an object. ([9118b53](https://www.github.com/JaCraig/Canister/commit/9118b53dadc760ecca17a58a7b3fbe059c0b8115))
* Fixing issue where Bootstrapper is destroyed and then reused. ([f4466ca](https://www.github.com/JaCraig/Canister/commit/f4466ca113c0be80cf8864b5e621b484f75d5e20))
* Fixing issue with ResolveAll. ([5daf27d](https://www.github.com/JaCraig/Canister/commit/5daf27dc491ada52b018f1c22ae5156045aaa091))
* Fixing some stuff. ([f54808a](https://www.github.com/JaCraig/Canister/commit/f54808a12f2e06db7e60eac03e8317508d577f92))
* Forgot about module resolution. ([caff382](https://www.github.com/JaCraig/Canister/commit/caff382339d2e5f5ad383ceef9e55becd3b8b7a8))
* Forgot the new .net core 2 app for testing. ([d1a927a](https://www.github.com/JaCraig/Canister/commit/d1a927ad0184fb4d18040e301d683ad2a47e36af))
* Helps if I actually use the base class... ([a7da576](https://www.github.com/JaCraig/Canister/commit/a7da576cff1857e6ab47f4ea58e571abb68c3369))
* Initial commit ([8c9b38d](https://www.github.com/JaCraig/Canister/commit/8c9b38de38b02f182728924e8d75c76c3eae7e68))
* Made some changes to the bootstrapper base class. Now has before/after build steps. Also instance objects are, by default, singletons. ([95495fb](https://www.github.com/JaCraig/Canister/commit/95495fb26f25f6fb419f7185f27ca655880b3cdf))
* Making it so that you can actually register open generic types... ([c0ec0b1](https://www.github.com/JaCraig/Canister/commit/c0ec0b155e971cd8c35c503b508b63357e36114a))
* merge ([a1bcf09](https://www.github.com/JaCraig/Canister/commit/a1bcf09593e1732702f009a254dd01174d13046c))
* Merge branch 'master' of https://github.com/JaCraig/Canister ([58b4e08](https://www.github.com/JaCraig/Canister/commit/58b4e08df77d228c659d76ce4f562dc3e63c77bf))
* Merge pull request #10 from JaCraig/dependabot/nuget/SimpleMVCTests/Microsoft.AspNetCore.All-2.1.4 ([2980ede](https://www.github.com/JaCraig/Canister/commit/2980edee1777ea986427276aff475d197c7e12a0))
* Merge pull request #11 from JaCraig/dependabot/nuget/SimpleMVCTests/Microsoft.AspNetCore.All-2.1.15 ([6fad77b](https://www.github.com/JaCraig/Canister/commit/6fad77bfa37ab8fe695d636c65ae5f5ae619e205))
* Merge with MVC integration ([c79ce37](https://www.github.com/JaCraig/Canister/commit/c79ce37dcb7906fcf3599a0fec7f1cf9274dab8f))
* Minor code cleanup. ([c7cc600](https://www.github.com/JaCraig/Canister/commit/c7cc6009b98064266290ee09a99592a22977e508))
* More fixing of tests. ([25b368a](https://www.github.com/JaCraig/Canister/commit/25b368a0e2de9a5960b7b84b0a82447c9536ca58))
* Package info updated. ([0090ad3](https://www.github.com/JaCraig/Canister/commit/0090ad37bb97526cad0eff0b69f0d333399686b5))
* Package updates. ([3f6d0e5](https://www.github.com/JaCraig/Canister/commit/3f6d0e5ab02b3208c24e5454bdf0ae23655c7684))
* Package updates. ([c2b3b3b](https://www.github.com/JaCraig/Canister/commit/c2b3b3b39ebd490469ea6690ca37f1ffff64b42c))
* Package updates. ([e212052](https://www.github.com/JaCraig/Canister/commit/e212052601efeb33b133fdf288e04834ef8348d3))
* Package updates. ([1fdc437](https://www.github.com/JaCraig/Canister/commit/1fdc4370bc8cfa93be85843327156661ad2d2c9f))
* Readme update. ([c6b11b1](https://www.github.com/JaCraig/Canister/commit/c6b11b143ac88e39d17a2161fcb7ce87078ad905))
* Realized that I had the wrong package name in the ReadMe... ([7ec0ff0](https://www.github.com/JaCraig/Canister/commit/7ec0ff081357975406a1d04d87b83e860b02907d))
* Removed old IoC container code. ([f6ab980](https://www.github.com/JaCraig/Canister/commit/f6ab980e7a6da55e330b365dec4e3c9fade50a14))
* Removing MVC test project until appveyor supports 2.1. ([2c25bec](https://www.github.com/JaCraig/Canister/commit/2c25becebb7ba829c0fa0a79c6ee36b15a7834a7))
* Removing unnecessary dependency. ([2d3ecd6](https://www.github.com/JaCraig/Canister/commit/2d3ecd6bf1cf57661a557d8621510fd8f8365742))
* Renaming the project to make it able to upload to nuget. ([939a22f](https://www.github.com/JaCraig/Canister/commit/939a22f511cfff5ed8417650a3cdc81c15364861))
* Since using GetRequiredService, no real point in checking for null. ([67f6e8c](https://www.github.com/JaCraig/Canister/commit/67f6e8c517e3be5db9be160b487bd79d2c176fda))
* Slight fix for ServiceDescriptor being blank. ([1af13db](https://www.github.com/JaCraig/Canister/commit/1af13dbd95c6a86cad851de7dd36961c83270e26))
* Small change to improve memory usage. ([068d869](https://www.github.com/JaCraig/Canister/commit/068d869bc4b5ffd35d0d96d0295a8ec19cb5864f))
* Small changes. ([088cdc0](https://www.github.com/JaCraig/Canister/commit/088cdc02385ea5531645d416b117cb6453b30f73))
* Small code cleanup. ([45181cf](https://www.github.com/JaCraig/Canister/commit/45181cfe22b6a5f0fbbf3c5ee869c501999e862f))
* Some basic code cleanup. ([c9e4c32](https://www.github.com/JaCraig/Canister/commit/c9e4c3202ce1b1fa75bd8bb3a43ee454d41fd9a7))
* Some small changes to simplify some code. ([1154aa5](https://www.github.com/JaCraig/Canister/commit/1154aa59b58549af552ce1bfd304815d070dd7e5))
* Something changed between 1.X and 2.X in ASP.Net around the authentication handler provider and the auth 2.0 checkin. Now they're using WindowsIdentity and closing the handle instead of the original ClaimsPrincipal object that was passed in. Closing the handle is the issue as it seems to happen more than once. As such using Canister with MVC seems to be fubar at the moment. Will revisit when I do the 2.0 version update. ([43aae79](https://www.github.com/JaCraig/Canister/commit/43aae795397a42fb2293016350e8b8a67f2122d0))
* Starting to work on integration with MVC's IoC. ([866f65c](https://www.github.com/JaCraig/Canister/commit/866f65ca589d91d8401d9321c8a098e33a801d79))
* Switched what portion of the library was registering the modules. Now a portion of the bootstrapper itself. ([99f2c37](https://www.github.com/JaCraig/Canister/commit/99f2c37be758456b29afcd6ce08c65f4232870ef))
* The assemblies used to configure the IoC container are now registered so that they can be pulled easily if needed by external systems. ([b051cf5](https://www.github.com/JaCraig/Canister/commit/b051cf516974baaadaa51c89895ce0d270c72dab))
* Trying to generate package. ([3b683f3](https://www.github.com/JaCraig/Canister/commit/3b683f36a0df8a3c62cf5300778dcd4b17d3990c))
* Update in case people weren't feeding the container a service collection. ([86546cd](https://www.github.com/JaCraig/Canister/commit/86546cd04ec099ca463741f636645238040baf58))
* Update README.md ([dac1780](https://www.github.com/JaCraig/Canister/commit/dac17800c18edd657bb2ff6a97c25e982400013d))
* Update README.md ([a1ec06c](https://www.github.com/JaCraig/Canister/commit/a1ec06c7a8d1c700d0d3f77e26480da25f1593c6))
* Updated dependencies for Canister. ([023be81](https://www.github.com/JaCraig/Canister/commit/023be81f17ef718b04c2f1f9dc5ed858f0675ac6))
* Updated to VS 2017. ([9851468](https://www.github.com/JaCraig/Canister/commit/985146804971bfe39a0b168357cc65dcf2906a8b))
* Updating package name because the original isn't available. ([e5a3e18](https://www.github.com/JaCraig/Canister/commit/e5a3e1814a47e593ed7ac8f283d591342b2fbdf3))
* Updating project to .Net Standard 2.1 ([6cdc24a](https://www.github.com/JaCraig/Canister/commit/6cdc24a20518a773a4129a243e2adfeb10186650))
* Updating readme. ([d825e69](https://www.github.com/JaCraig/Canister/commit/d825e692131b12a8ba80614f1758bccb129bf5a0))
* Updating references. ([c963989](https://www.github.com/JaCraig/Canister/commit/c963989a305b4c4f6ebc6ea1dcafe2854ba2496f))
* Updating to .net standard 2.0. ([4f2a18d](https://www.github.com/JaCraig/Canister/commit/4f2a18df60d45d6792f1b7de989ff824674adeda))


extern "C" {
    void __iOSOpenPhotoAlbum() {
        NSLog(@"打开相册");
    }

    void __iOSOpenWebPage(const char* url) {
        NSString* urlStr = [NSString stringWithUTF8String:url];
        [[UIApplication sharedApplication] openURL:[NSURL URLWithString:urlStr] options:@{} completionHandler:nil];
    }
}
